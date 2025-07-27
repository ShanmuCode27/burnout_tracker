using BurnoutTracker.Application.Dtos;
using BurnoutTracker.Domain.Models.Entities;
using BurnoutTracker.Infrastructure;
using BurnoutTracker.Utilities;
using Microsoft.EntityFrameworkCore;

namespace BurnoutTracker.Application.Services
{
    public interface IBurnoutDetectionService
    {
        Task ProcessBurnoutStatesForAllConnectionsAsync();
        Task ProcessBurnoutStatesForConnectionsAsync(UserRepositoryConnection connection);
        Task<List<DeveloperBurnoutState>> GetLatestDevState(long connectionId);
        Task<List<DeveloperBurnoutState>> GetDevHistory(long connectionId);
        Task<List<CommitStatsDto>> GetCommitStats(long connectionId, int days = 30);
        Task<List<DeveloperActivityDto>> GetDeveloperStatsAsync(long repoConnectionId, long userId);
        Task<DeveloperDetailDto?> GetDeveloperDetailAsync(long connectionId, long userId, string login);
    }

    public class BurnoutDetectionService: IBurnoutDetectionService
    {
        private readonly IRepositoryPlatformDispatcherService _dispatcher;
        private readonly BTDbContext _db;
        private readonly DeveloperMetricsCalculator _developerMetricsCalculator;

        public BurnoutDetectionService(IRepositoryPlatformDispatcherService dispatcher, BTDbContext db, DeveloperMetricsCalculator developerMetricsCalculator)
        {
            _dispatcher = dispatcher;
            _db = db;
            _developerMetricsCalculator = developerMetricsCalculator;
        }

        public async Task ProcessBurnoutStatesForAllConnectionsAsync()
        {
            var connections = await _db.UserRepositoryConnections.ToListAsync();
            await ProcessRepoSync(connections);
        }
        public async Task ProcessBurnoutStatesForConnectionsAsync(UserRepositoryConnection connection)
        {
            var connections = new List<UserRepositoryConnection> { connection };
            await ProcessRepoSync(connections);
        }


            public async Task<List<DeveloperBurnoutState>> GetLatestDevState(long connectionId)
        {
            var latest = await _db.DeveloperBurnoutStates
                .Where(s => s.UserRepositoryConnectionId == connectionId)
                .GroupBy(s => s.DeveloperLogin)
                .Select(g => g.OrderByDescending(s => s.RecordedAt).First())
                .ToListAsync();

            return latest;
        }

        public async Task<List<DeveloperBurnoutState>> GetDevHistory(long connectionId)
        {
            var history = await _db.DeveloperBurnoutStates
                .Where(s => s.UserRepositoryConnectionId == connectionId)
                .OrderByDescending(s => s.RecordedAt)
                .ToListAsync();

            return history;
        }

        public async Task<List<CommitStatsDto>> GetCommitStats(long connectionId, int days = 30)
        {
            var since = DateTime.UtcNow.AddDays(-days);
            var stats = await _db.DeveloperBurnoutStates
                .Where(s => s.UserRepositoryConnectionId == connectionId && s.RecordedAt >= since)
                .GroupBy(s => s.DeveloperLogin)
                .Select(g => new CommitStatsDto
                {
                    Developer = g.Key,
                    TotalCommits = g.Sum(s => s.WeeklyCommitCount),
                    AvgCommitsPerWeek = (int)g.Average(s => s.WeeklyCommitCount),
                    States = g.Select(x => new StateCountDto
                    {
                        RecordedAt = x.RecordedAt,
                        State = x.State
                    }).ToList()
                }).ToListAsync();

            return stats;
        }

        private async Task ProcessRepoSync(List<UserRepositoryConnection> connections)
       {
            foreach (var connection in connections)
            {
                var activity = await _dispatcher.GetDeveloperActivityAsync(connection);

                foreach (var dev in activity)
                {
                    var state = dev.WeeklyCommitCount switch
                    {
                        <= 30 => "Active",
                        <= 60 => "Warning",
                        _ => "BurnedOut"
                    };

                    var record = new DeveloperBurnoutState
                    {
                        UserRepositoryConnectionId = connection.Id,
                        DeveloperLogin = dev.DeveloperLogin,
                        State = state,
                        TotalCommitCount = dev.TotalCommitCount,
                        WeeklyCommitCount = dev.WeeklyCommitCount,
                        RecordedAt = DateTime.UtcNow
                    };

                    _db.DeveloperBurnoutStates.Add(record);
                }

                await _db.SaveChangesAsync();
            }
        }

        public async Task<List<DeveloperActivityDto>> GetDeveloperStatsAsync(long repoConnectionId, long userId)
        {
            var connection = await _db.UserRepositoryConnections
                .Include(r => r.SupportedRepository)
                .FirstOrDefaultAsync(r => r.Id == repoConnectionId && r.UserId == userId);

            if (connection == null)
                throw new Exception("Repository connection not found.");

            var gitService = await _dispatcher.GetGitRepositoryService(connection.SupportedRepositoryId);

            var (owner, repo) = StringHelper.ParseRepoUrl(connection.RepositoryUrl);
            var since = DateTime.UtcNow.AddDays(-30).ToString("yyyy-MM-ddTHH:mm:ssZ");

            var commits = await gitService.GetCommitsDetailedAsync(owner, repo, since, connection.AccessToken);
            var pullRequests = await gitService.GetPullRequestsAsync(owner, repo, connection.AccessToken);

            if (commits == null || pullRequests == null)
            {
                return [];
            }

            var devMetrics = _developerMetricsCalculator.Calculate(commits, pullRequests);
            return devMetrics;
        }

        public async Task<DeveloperDetailDto?> GetDeveloperDetailAsync(long connectionId, long userId, string login)
        {
            var connection = await _db.UserRepositoryConnections
                .Include(c => c.SupportedRepository)
                .FirstOrDefaultAsync(c => c.Id == connectionId && c.UserId == userId);

            if (connection == null) return null;

            var supportedApis = await _db.RepositoryApis
                .Where(api => api.SupportedRepositoryId == connection.SupportedRepositoryId)
                .ToListAsync();

            var gitService = await _dispatcher.GetGitRepositoryService(connection.SupportedRepositoryId);
            var (owner, repo) = StringHelper.ParseRepoUrl(connection.RepositoryUrl);
            var pullRequests = await gitService.GetPullRequestsAsync(owner, repo, connection.AccessToken);
            var since = DateTime.UtcNow.AddDays(-30).ToString("yyyy-MM-ddTHH:mm:ssZ");

            var commits = await gitService.GetCommitsDetailedAsync(owner, repo, since, connection.AccessToken);


            var devCommits = commits.Where(c => c.Author?.Login == login).ToList();
            if (!devCommits.Any()) return null;

            var metrics = _developerMetricsCalculator.Calculate(devCommits, pullRequests, login);

            DateTime? latestWorkTimeUtc = null;
            if (DateTime.TryParse(metrics.LatestWorkTimeUtc, out var parsedDate))
            {
                latestWorkTimeUtc = parsedDate;
            }

            return new DeveloperDetailDto
            {
                DeveloperLogin = login,
                WeeklyCommitCount = metrics.WeeklyCommitCount,
                TotalCommitCount = devCommits.Count,
                PullRequestCount = metrics.PullRequestCount,
                ReviewChangesCount = metrics.ReviewChangesCount,
                NightWorkCount = metrics.NightWorkCount,
                LatestWorkTimeUtc = latestWorkTimeUtc,
                BurnoutScore = metrics.BurnoutScore,
                BurnoutStatus = metrics.BurnoutStatus,
                RevertCount = metrics.RevertCount,
            };
        }

    }
}
