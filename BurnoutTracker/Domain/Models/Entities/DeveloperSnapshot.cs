namespace BurnoutTracker.Domain.Models.Entities
{
    public class DeveloperSnapshot
    {
        public long Id { get; set; }
        public long UserRepositoryConnectionId { get; set; }

        public string DeveloperLogin { get; set; } = string.Empty;
        public int WeeklyCommitCount { get; set; }
        public int TotalCommitCount { get; set; }
        public int PullRequestCount { get; set; }
        public int ReviewChangesCount { get; set; }
        public int NightWorkCount { get; set; }
        public string? LatestWorkTimeUtc { get; set; }

        public int BurnoutScore { get; set; }
        public string BurnoutStatus { get; set; } = "Unknown";

        public DateTime CapturedAt { get; set; } = DateTime.UtcNow;
    }

}
