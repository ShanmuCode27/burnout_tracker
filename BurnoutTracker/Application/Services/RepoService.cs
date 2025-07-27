using BurnoutTracker.Application.Dtos;
using BurnoutTracker.Domain.Models;
using BurnoutTracker.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BurnoutTracker.Application.Services
{
    public interface IRepoService
    {
        Task<List<ConnectedRepositoryDto>> GetConnectedRepositoriesAsync(long userId);
        Task ConnectRepositoryAsync(long userId, ConnectRepoRequestDto request);
    }


    public class RepoService : IRepoService
    {
        private readonly BTDbContext _db;

        public RepoService(BTDbContext context)
        {
            _db = context;
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
                    Platform = r.SupportedRepository.Name
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
                AccessToken = request.AccessToken
            };

            _db.UserRepositoryConnections.Add(connection);
            await _db.SaveChangesAsync();
        }

    }

}
