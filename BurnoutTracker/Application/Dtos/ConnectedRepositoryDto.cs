namespace BurnoutTracker.Application.Dtos
{
    public class ConnectedRepositoryDto
    {
        public long Id { get; set; }
        public string RepositoryUrl { get; set; } = string.Empty;
        public string Platform { get; set; } = string.Empty;
        public long SupportedRepositoryId { get; set; }
    }
}
