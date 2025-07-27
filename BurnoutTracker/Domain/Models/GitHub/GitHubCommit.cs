namespace BurnoutTracker.Domain.Models.GitHub
{
    public class GitHubCommit
    {
        public GitHubCommitAuthor Author { get; set; } = null!;
        public string Message { get; set; } = string.Empty;
    }

    public class GitHubCommitAuthor
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime Date { get; set; }
    }

    public class GitHubUser
    {
        public string Login { get; set; } = string.Empty;
    }

    public class GitHubStats
    {
        public int Additions { get; set; }
        public int Deletions { get; set; }
        public int Total { get; set; }
    }

    public class GitHubFile
    {
        public string Filename { get; set; } = string.Empty;
        public int Changes { get; set; }
        public int Additions { get; set; }
        public int Deletions { get; set; }
    }
}
