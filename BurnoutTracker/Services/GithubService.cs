using BurnoutTracker.Dtos;
using BurnoutTracker.Infrastructure;
using BurnoutTracker.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Text.Json;

namespace BurnoutTracker.Services
{
    public interface IGitHubService
    {
        Task SaveUserTokenAsync(long userId, string token);
        Task<List<RepoDto>> GetUserReposAsync(long userId);
    }


    public class GitHubService : IGitHubService
    {
        private readonly BTDbContext _db;
        private readonly HttpClient _client;

        public GitHubService(BTDbContext db, IHttpClientFactory factory)
        {
            _db = db;
            _client = factory.CreateClient("GitHub");
        }

        public async Task SaveUserTokenAsync(long userId, string token)
        {
            var existing = await _db.GitHubTokens.FirstOrDefaultAsync(t => t.UserId == userId);
            if (existing != null)
            {
                existing.Token = token;
            }
            else
            {
                await _db.GitHubTokens.AddAsync(new GithubToken { UserId = userId, Token = token });
            }

            await _db.SaveChangesAsync();
        }

        public async Task<List<RepoDto>> GetUserReposAsync(long userId)
        {
            var token = await _db.GitHubTokens
                .Where(t => t.UserId == userId)
                .Select(t => t.Token)
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
    }
}
