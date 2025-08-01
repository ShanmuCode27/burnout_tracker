using BurnoutTracker.Domain.Models.Entities;
using BurnoutTracker.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BurnoutTracker.Application.Services
{
    public interface IUserService
    {
        Task<User?> GetUserByUsernameAsync(string username);
        Task<User> RegisterAsync(string username, string password);
        Task<bool> ValidateCredentialsAsync(string username, string password);
    }

    public class UserService : IUserService
    {
        private readonly BTDbContext _db;

        public UserService(BTDbContext db)
        {
            _db = db;
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User> RegisterAsync(string username, string password)
        {
            var existing = await GetUserByUsernameAsync(username);
            if (existing != null)
                throw new InvalidOperationException("User already exists");

            var hash = BCrypt.Net.BCrypt.HashPassword(password);
            var user = new User { Username = username, PasswordHash = hash };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return user;
        }

        public async Task<bool> ValidateCredentialsAsync(string username, string password)
        {
            var user = await GetUserByUsernameAsync(username);
            if (user == null) return false;
            return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        }

    }
}
