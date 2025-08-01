namespace BurnoutTracker.Domain.Models.Entities
{
    public class UserRepositoryConnection
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public User User { get; set; } = null!;
        public string RepositoryUrl { get; set; } = string.Empty;
        public long SupportedRepositoryId { get; set; }
        public SupportedRepository SupportedRepository { get; set; } = null!;
        public string? AccessToken { get; set; }
        public DateTime ConnectedAt { get; set; } = DateTime.UtcNow;
        public string Branch { get; set; } = "main";
    }

}
