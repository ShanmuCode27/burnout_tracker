using BurnoutTracker.Application.Dtos;
using BurnoutTracker.Domain.Models.Entities;
using BurnoutTracker.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BurnoutTracker.Application.Services
{
    public interface IRepoService
    {
        Task<List<ConnectedRepositoryDto>> GetConnectedRepositoriesAsync(long userId);
        Task ConnectRepositoryAsync(long userId, ConnectRepoRequestDto request);
        Task DeleteRepository(long connectionId, long userId);
    }

    public class RepoService : IRepoService
    {
        private readonly BTDbContext _db;
        private readonly IBurnoutDetectionService _burnoutDetectionService;

        public RepoService(BTDbContext context, IBurnoutDetectionService burnoutDetectionService)
        {
            _db = context;
            _burnoutDetectionService = burnoutDetectionService;
        }

        public async Task<List<ConnectedRepositoryDto>> GetConnectedRepositoriesAsync(long userId)
        {
            return await _db.UserRepositoryConnections
                .Include(r => r.SupportedRepository)
                .Where(r => r.UserId == userId)
                .Select(r => new ConnectedRepositoryDto
                {
                    Id = r.Id,
                    RepositoryUrl = r.RepositoryUrl,
                    Platform = r.SupportedRepository.Name,
                    SupportedRepositoryId = r.SupportedRepositoryId
                })
                .ToListAsync();
        }

        public async Task ConnectRepositoryAsync(long userId, ConnectRepoRequestDto request)
        {
            var alreadyConnected = await _db.UserRepositoryConnections
                .AnyAsync(c =>
                    c.UserId == userId &&
                    c.RepositoryUrl == request.RepositoryUrl &&
                    c.SupportedRepositoryId == request.SupportedRepositoryId);

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id.Equals(userId));
            var repo = await _db.SupportedRepositories
                .AsNoTracking()
                .FirstOrDefaultAsync(sr => sr.Id.Equals(request.SupportedRepositoryId));

            if (alreadyConnected || repo == null || user == null)
                return;

            var connection = new UserRepositoryConnection
            {
                UserId = user.Id,
                RepositoryUrl = request.RepositoryUrl,
                SupportedRepositoryId = repo.Id,
                AccessToken = request.AccessToken,
                Branch = request.Branch
            };

            _db.UserRepositoryConnections.Add(connection);
            await _db.SaveChangesAsync();

            await _burnoutDetectionService.ProcessBurnoutStatesForConnectionsAsync(connection);
        }

        public async Task DeleteRepository(long connectionId, long userId)
        {
            var connection = await _db.UserRepositoryConnections
                .FirstOrDefaultAsync(c => c.Id == connectionId && c.UserId == userId);

            if (connection == null)
            {
                //TODO: handle
            }

            var developerStates = await _db.DeveloperBurnoutStates.Where(dbs => Equals(dbs.UserRepositoryConnectionId, connectionId)).ToListAsync();
            _db.DeveloperBurnoutStates.RemoveRange(developerStates);

            _db.UserRepositoryConnections.Remove(connection);
            await _db.SaveChangesAsync();
        }
    }
}
