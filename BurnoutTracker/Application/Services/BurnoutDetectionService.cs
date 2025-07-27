using BurnoutTracker.Application.Dtos;
using BurnoutTracker.Domain.Models.Entities;
using BurnoutTracker.Infrastructure;
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
    }

    public class BurnoutDetectionService: IBurnoutDetectionService
    {
        private readonly IRepositoryPlatformDispatcherService _dispatcher;
        private readonly BTDbContext _db;

        public BurnoutDetectionService(IRepositoryPlatformDispatcherService dispatcher, BTDbContext db)
        {
            _dispatcher = dispatcher;
            _db = db;
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
                        >= 3 => "Active",
                        >= 1 => "Warning",
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
    }

}
