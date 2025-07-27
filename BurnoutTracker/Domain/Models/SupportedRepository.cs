namespace BurnoutTracker.Domain.Models
{
    public class SupportedRepository
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty; // e.g., "GitHub"
        public string BaseUrl { get; set; } = string.Empty; // e.g., "https://api.github.com"

        public ICollection<RepositoryApi> Endpoints { get; set; } = new List<RepositoryApi>();
    }
}
