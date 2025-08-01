namespace BurnoutTracker.Application.Dtos
{
    public class DeveloperActivityDto
    {
        public string DeveloperLogin { get; set; } = string.Empty;
        public int WeeklyCommitCount { get; set; }
        public int TotalCommitCount { get; set; }
        public int PullRequestCount { get; set; }
        public int ReviewChangesCount { get; set; }
        public int NightWorkCount { get; set; }
        public string? LatestWorkTimeUtc { get; set; }

        public int BurnoutScore { get; set; }
        public string BurnoutStatus { get; set; } = "Unknown"; // Active, Warning, BurnedOut
    }
}
