namespace BurnoutTracker.Domain.Models.Entities
{
    public class RepositoryApi
    {
        public int Id { get; set; }
        public int SupportedRepositoryId { get; set; }
        public SupportedRepository SupportedRepository { get; set; } = null!;
        public string Name { get; set; } = string.Empty; // e.g., "Get Contributors"
        public string Path { get; set; } = string.Empty; // e.g., "/repos/{owner}/{repo}/contributors"
        public string Method { get; set; } = "GET";
    }
}
