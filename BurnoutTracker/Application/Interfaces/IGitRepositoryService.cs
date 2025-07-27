using BurnoutTracker.Application.Dtos;
using BurnoutTracker.Domain.Models.Entities;

namespace BurnoutTracker.Application.Interfaces
{
    public interface IGitRepositoryService
    {
        Task SaveUserTokenAsync(long userId, string token, long repositoryTypeId);
        Task<List<RepoDto>> GetUserReposAsync(long userId);
        Task<List<ContributorDto>> GetContributorsAsync(string owner, string repo, string? token = null);
        Task<List<DeveloperActivityDto>> GetDeveloperActivityAsync(string repositoryUrl, string? accessToken, string branch, List<RepositoryApi> supportedApis);
    }
}
