using BurnoutTracker.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace BurnoutTracker.Infrastructure
{
    public class BTDbContext: DbContext
    {
        public BTDbContext(DbContextOptions<BTDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<GithubToken> GitHubTokens => Set<GithubToken>();
    }
}
