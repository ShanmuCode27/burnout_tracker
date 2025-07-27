using BurnoutTracker.Domain.Models.Entities;
using BurnoutTracker.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BurnoutTracker.Application.Services
{
    public class BurnoutDetectionService
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
