using BurnoutTracker.Application.Dtos;
using BurnoutTracker.Domain.Models.Entities;
using BurnoutTracker.Infrastructure;
using BurnoutTracker.Utilities;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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

                var gitService = await _dispatcher.GetGitRepositoryService(connection.SupportedRepositoryId);

                var (owner, repo) = StringHelper.ParseRepoUrl(connection.RepositoryUrl);
                var since = DateTime.UtcNow.AddDays(-30).ToString("yyyy-MM-ddTHH:mm:ssZ");

                var commits = await gitService.GetCommitsDetailedAsync(owner, repo, since, connection.AccessToken);
                var pullRequests = await gitService.GetPullRequestsAsync(owner, repo, connection.AccessToken);

                foreach (var dev in activity)
                {
                    var metrics = _developerMetricsCalculator.Calculate(commits, pullRequests, dev.DeveloperLogin);

                    DateTime? latestWorkTimeUtc = null;
                    if (DateTime.TryParse(metrics.LatestWorkTimeUtc, out var parsedDate))
                    {
                        latestWorkTimeUtc = parsedDate;
                    }
                    var existingData = await _db.DeveloperBurnoutStates
                        .FirstOrDefaultAsync(x => x.UserRepositoryConnectionId == connection.Id && x.DeveloperLogin == dev.DeveloperLogin);

                    if (existingData != null)
                    {
                        existingData.State = metrics.BurnoutStatus;
                        existingData.TotalCommitCount = dev.TotalCommitCount;
                        existingData.WeeklyCommitCount = dev.WeeklyCommitCount;
                        existingData.RecordedAt = DateTime.UtcNow;
                        existingData.PullRequestCount = dev.PullRequestCount;
                        existingData.ReviewChangesCount = dev.ReviewChangesCount;
                        existingData.NightWorkCount = dev.NightWorkCount;
                        existingData.LatestWorkTimeUtc = latestWorkTimeUtc;
                        existingData.RevertCount = metrics.RevertCount;
                        existingData.BurnoutScore = metrics.BurnoutScore;

                        _db.DeveloperBurnoutStates.Update(existingData);
                    } else
                    {
                        var record = new DeveloperBurnoutState
                        {
                            UserRepositoryConnectionId = connection.Id,
                            DeveloperLogin = dev.DeveloperLogin,
                            State = metrics.BurnoutStatus,
                            TotalCommitCount = dev.TotalCommitCount,
                            WeeklyCommitCount = dev.WeeklyCommitCount,
                            RecordedAt = DateTime.UtcNow,
                            PullRequestCount = dev.PullRequestCount,
                            ReviewChangesCount = dev.ReviewChangesCount,
                            NightWorkCount = dev.NightWorkCount,
                            LatestWorkTimeUtc = latestWorkTimeUtc,
                            RevertCount = metrics.RevertCount,
                            BurnoutScore = metrics.BurnoutScore
                        };

                        await _db.DeveloperBurnoutStates.AddAsync(record);
                    }
                }

                await _db.SaveChangesAsync();
            }
        }

        public async Task<List<DeveloperActivityDto>> GetDeveloperStatsAsync(long repoConnectionId, long userId)
        {
            var developersInfo = await _db.DeveloperBurnoutStates
                .Where(r => r.UserRepositoryConnectionId == repoConnectionId)
                .ToListAsync();

            if (developersInfo == null)
                throw new Exception("Repository or developer info not found.");

            var devActivities = new List<DeveloperActivityDto>();

            foreach (var developerInfo in developersInfo)
            {
                var devActivity = new DeveloperActivityDto
                {
                    DeveloperLogin = developerInfo.DeveloperLogin,
                    WeeklyCommitCount = developerInfo.WeeklyCommitCount,
                    TotalCommitCount = developerInfo.TotalCommitCount,
                    PullRequestCount = developerInfo.PullRequestCount,
                    ReviewChangesCount = developerInfo.ReviewChangesCount,
                    NightWorkCount = developerInfo.NightWorkCount,
                    LatestWorkTimeUtc = developerInfo.LatestWorkTimeUtc.ToString(),
                    BurnoutScore = developerInfo.BurnoutScore,
                    BurnoutStatus = developerInfo.State
                };

                devActivities.Add(devActivity);
            }

            return devActivities;
        }

        public async Task<DeveloperDetailDto?> GetDeveloperDetailAsync(long connectionId, long userId, string login)
        {
            var developerInfo = await _db.DeveloperBurnoutStates
                .FirstOrDefaultAsync(
                    c => c.UserRepositoryConnectionId == connectionId && 
                    c.DeveloperLogin.Equals(login));

            if (developerInfo == null) return null;


            return new DeveloperDetailDto
            {
                DeveloperLogin = login,
                WeeklyCommitCount = developerInfo.WeeklyCommitCount,
                TotalCommitCount = developerInfo.TotalCommitCount,
                PullRequestCount = developerInfo.PullRequestCount,
                ReviewChangesCount = developerInfo.ReviewChangesCount,
                NightWorkCount = developerInfo.NightWorkCount,
                LatestWorkTimeUtc = developerInfo.LatestWorkTimeUtc,
                BurnoutScore = developerInfo.BurnoutScore,
                BurnoutStatus = developerInfo.State,
                RevertCount = developerInfo.RevertCount
            };
        }

    }
}
