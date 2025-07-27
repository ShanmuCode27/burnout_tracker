using BurnoutTracker.Domain.Models.GitHub;

namespace BurnoutTracker.Application.Dtos.Github
{
    public class GitHubCommitDto
    {
        public string Sha { get; set; } = string.Empty;
        public GitHubCommit Commit { get; set; } = null!;
        public GitHubUser? Author { get; set; }
        public List<GitHubFile>? Files { get; set; }
        public GitHubStats? Stats { get; set; }
    }
}
