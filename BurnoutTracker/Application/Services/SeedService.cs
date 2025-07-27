using BurnoutTracker.Domain.Models;
using BurnoutTracker.Infrastructure;

namespace BurnoutTracker.Application.Services
{
    public class SeedService
    {
        private readonly BTDbContext _context;

        public SeedService(BTDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            if (!_context.SupportedRepositories.Any())
            {
                var githubRepo = new SupportedRepository
                {
                    Name = "GitHub",
                    BaseUrl = "https://api.github.com"
                };

                _context.SupportedRepositories.Add(githubRepo);

                _context.RepositoryApis.AddRange(
                    new RepositoryApi
                    {
                        SupportedRepository = githubRepo,
                        Name = "Get Contributors",
                        Path = "/repos/{owner}/{repo}/contributors",
                        Method = "GET"
                    },
                    new RepositoryApi
                    {
                        SupportedRepository = githubRepo,
                        Name = "Get Commits",
                        Path = "/repos/{owner}/{repo}/commits",
                        Method = "GET"
                    },
                    new RepositoryApi
                    {
                        SupportedRepository = githubRepo,
                        Name = "Get Pulls",
                        Path = "/repos/{owner}/{repo}/pulls?state=all",
                        Method = "GET"
                    },
                    new RepositoryApi
                    {
                        SupportedRepository = githubRepo,
                        Name = "Get Issues",
                        Path = "/repos/{owner}/{repo}/issues?state=all",
                        Method = "GET"
                    },
                    new RepositoryApi
                    {
                        SupportedRepository = githubRepo,
                        Name = "Get Commit Activity",
                        Path = "/repos/{owner}/{repo}/stats/commit_activity",
                        Method = "GET"
                    },
                    new RepositoryApi
                    {
                        SupportedRepository = githubRepo,
                        Name = "Get Participation",
                        Path = "/repos/{owner}/{repo}/stats/participation",
                        Method = "GET"
                    },
                    new RepositoryApi
                    {
                        SupportedRepository = githubRepo,
                        Name = "Get Contributor Statis",
                        Path = "/repos/{owner}/{repo}/stats/contributors",
                        Method = "GET"
                    }
                );

                await _context.SaveChangesAsync();
            }
        }
    }

}
