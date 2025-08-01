namespace BurnoutTracker.Application.Dtos
{
    public class ConnectRepoRequestDto
    {
        public string RepositoryUrl { get; set; } = string.Empty;
        public string? AccessToken { get; set; }
        public long SupportedRepositoryId { get; set; }
        public string Branch { get; set; } = "main";
    }
}
