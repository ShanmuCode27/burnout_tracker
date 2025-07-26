using BurnoutTracker.Application.Dtos;
using BurnoutTracker.Infrastructure;
using BurnoutTracker.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Text.Json;

namespace BurnoutTracker.Application.Services
{
    public interface IGitHubService
    {
        Task SaveUserTokenAsync(long userId, string token, long repositoryTypeId);
        Task<List<RepoDto>> GetUserReposAsync(long userId);
        Task<List<ContributorDto>> GetContributorsAsync(string owner, string repo, string? token = null);
    }


    public class GitHubService : IGitHubService
    {
        private readonly BTDbContext _db;
        private readonly HttpClient _client;
        private readonly ILogger _logger;

        public GitHubService(BTDbContext db, IHttpClientFactory factory, ILogger logger)
        {
            _db = db;
            _client = factory.CreateClient("GitHub");
            _logger = logger;
        }

        public async Task SaveUserTokenAsync(long userId, string token, long repositoryTypeId)
        {
            var existing = await _db.UserRepositoryConnections.FirstOrDefaultAsync(
                t => t.UserId == userId && 
                Equals(t.SupportedRepositoryId, repositoryTypeId));
            if (existing != null)
            {
                existing.AccessToken = token;
            }
            else
            {
                await _db.UserRepositoryConnections.AddAsync(new UserRepositoryConnection { UserId = userId, AccessToken = token });
            }

            await _db.SaveChangesAsync();
        }

        public async Task<List<RepoDto>> GetUserReposAsync(long userId)
        {
            var token = await _db.UserRepositoryConnections
                .Where(t => t.UserId == userId)
                .Select(t => t.AccessToken)
                .FirstOrDefaultAsync();

            if (token == null) throw new UnauthorizedAccessException("GitHub token missing");

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("token", token);

            var response = await _client.GetAsync("https://api.github.com/user/repos");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<RepoDto>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<RepoDto>();
        }

        public async Task<List<ContributorDto>> GetContributorsAsync(string owner, string repo, string? token = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.github.com/repos/{owner}/{repo}/contributors");
            request.Headers.UserAgent.ParseAdd("burnout-tracker-app");

            if (!string.IsNullOrEmpty(token))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogError("GitHub API error: {StatusCode} {Message}", response.StatusCode, content);
                throw new Exception("Failed to fetch contributors");
            }

            var json = await response.Content.ReadAsStringAsync();
            var contributors = JsonSerializer.Deserialize<List<ContributorDto>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return contributors ?? new List<ContributorDto>();
        }
    }
}
