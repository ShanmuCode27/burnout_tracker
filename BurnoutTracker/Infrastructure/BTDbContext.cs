using BurnoutTracker.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace BurnoutTracker.Infrastructure
{
    public class BTDbContext: DbContext
    {
        public BTDbContext(DbContextOptions<BTDbContext> options) : base(options) { }



        public DbSet<User> Users => Set<User>();
        public DbSet<SupportedRepository> SupportedRepositories => Set<SupportedRepository>();
        public DbSet<RepositoryApi> RepositoryApis => Set<RepositoryApi>();
        public DbSet<UserRepositoryConnection> UserRepositoryConnections => Set<UserRepositoryConnection>();
    }
}
