namespace BurnoutTracker.Application.Dtos.Github
{
    public class GitHubPullRequestDto
    {
        public int Number { get; set; }
        public GitHubUser User { get; set; } = new();
        public int Comments { get; set; }
        public int ReviewComments { get; set; }
        public int Commits { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
