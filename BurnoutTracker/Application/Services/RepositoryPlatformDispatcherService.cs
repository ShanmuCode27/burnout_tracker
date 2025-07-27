using BurnoutTracker.Application.Dtos;
using BurnoutTracker.Application.Interfaces;
using BurnoutTracker.Domain.Models.Entities;
using BurnoutTracker.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BurnoutTracker.Application.Services
{
    public interface IRepositoryPlatformDispatcherService
    {
        Task<List<DeveloperActivityDto>> GetDeveloperActivityAsync(UserRepositoryConnection connection);
    }

    public class RepositoryPlatformDispatcherService: IRepositoryPlatformDispatcherService
    {
        private readonly BTDbContext _db;
        private readonly IGitHubRepositoryPlatformService _gitRepositoryPlatformService;
        private readonly Dictionary<string, IGitRepositoryService> _platformServices;

        public RepositoryPlatformDispatcherService(BTDbContext db, IGitHubRepositoryPlatformService githubService)
        {
            _db = db;
            _platformServices = new Dictionary<string, IGitRepositoryService> 
            {
                { "github", githubService }
            };
        }

        public async Task<List<DeveloperActivityDto>> GetDeveloperActivityAsync(UserRepositoryConnection connection)
        {
            var supportedRepo = await _db.SupportedRepositories
                .Include(r => r.Endpoints)
                .FirstOrDefaultAsync(r => r.Id == connection.SupportedRepositoryId);

            if (supportedRepo == null)
                throw new Exception("Unsupported repository platform");

            if (!_platformServices.TryGetValue(supportedRepo.Name, out var service))
                throw new NotImplementedException($"Platform '{supportedRepo.Name}' not supported");

            return await service.GetDeveloperActivityAsync(
                connection.RepositoryUrl,
                connection.AccessToken,
                supportedRepo.Endpoints.ToList()
            );
        }
    }
}
