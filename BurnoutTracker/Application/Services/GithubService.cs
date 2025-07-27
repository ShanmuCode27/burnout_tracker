using BurnoutTracker.Application.Dtos;
using BurnoutTracker.Infrastructure;
using BurnoutTracker.Domain.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Text.Json;
using BurnoutTracker.Application.Dtos.Github;
using BurnoutTracker.Application.Interfaces;

namespace BurnoutTracker.Application.Services
{
    public interface IGitHubRepositoryPlatformService: IGitRepositoryService
    {
        string PlatformName { get; }
    }


    public class GitHubService : IGitHubRepositoryPlatformService
    {
        private readonly BTDbContext _db;
        private readonly HttpClient _client;
        private readonly ILogger<GitHubService> _logger;

        public string PlatformName => "GithubService";

        public GitHubService(BTDbContext db, IHttpClientFactory factory, ILogger<GitHubService> logger)
        {
            _db = db;
            _client = factory.CreateClient("GitHub");
            _client.BaseAddress = new Uri("https://api.github.com/");
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
                await _db.UserRepositoryConnections.AddAsync(new UserRepositoryConnection { UserId = userId, AccessToken = token, SupportedRepositoryId = repositoryTypeId });
            }

            await _db.SaveChangesAsync();
        }

        public async Task<List<RepoDto>> GetUserReposAsync(long userId)
        {
            var token = await _db.UserRepositoryConnections
                .Where(t => t.UserId == userId)
                .Select(t => t.AccessToken)
                .FirstOrDefaultAsync();

            if (token == null) throw new UnauthorizedAccessException("Repository access token missing");

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("token", token);

            var response = await _client.GetAsync("user/repos");
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


        public async Task<List<DeveloperActivityDto>> GetDeveloperActivityAsync(string repositoryUrl, string? accessToken, string branch, List<RepositoryApi> supportedApis)
        {
            var commitsApi = supportedApis.FirstOrDefault(api => api.Name.Equals("Get Commits", StringComparison.OrdinalIgnoreCase));
            if (commitsApi == null) return new();

            var (owner, repo) = ParseRepoUrl(repositoryUrl);
            var since = DateTime.UtcNow.AddDays(-30).ToString("yyyy-MM-ddTHH:mm:ssZ");

            var url = commitsApi.Path
                .Replace("{owner}", owner)
                .Replace("{repo}", repo)
                .Replace("{since}", since);

            if (url.Contains("?"))
            {
                url = url + $"&sha={branch}";
            } else
            {
                url = url + $"?sha={branch}";
            }

            _client.DefaultRequestHeaders.UserAgent.ParseAdd("burnout-tracker-app");
            if (!string.IsNullOrEmpty(accessToken))
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            try
            {
                var response = await _client.GetAsync(url);
                if (!response.IsSuccessStatusCode) return new();

                var data = await response.Content.ReadFromJsonAsync<List<GitHubCommitDto>>() ?? new();

                return data
                    .Where(c => c.Author?.Login != null)
                    .GroupBy(c => c.Author!.Login)
                    .Select(g => new DeveloperActivityDto
                    {
                        DeveloperLogin = g.Key,
                        WeeklyCommitCount = g.Count() / 4,
                        TotalCommitCount = g.Count()
                    }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errr");
                return new();
            }

        }

        private (string owner, string repo) ParseRepoUrl(string url)
        {
            // Parse https://github.com/{owner}/{repo}
            var parts = new Uri(url).AbsolutePath.Trim('/').Split('/');
            return (parts[0], parts[1]);
        }
    }
}
