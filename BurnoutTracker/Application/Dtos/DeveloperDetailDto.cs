namespace BurnoutTracker.Application.Dtos
{
    public class DeveloperDetailDto
    {
        public string DeveloperLogin { get; set; } = string.Empty;

        public int WeeklyCommitCount { get; set; }
        public int TotalCommitCount { get; set; }

        public int PullRequestCount { get; set; }
        public int ReviewChangesCount { get; set; }

        public int NightWorkCount { get; set; }
        public DateTime? LatestWorkTimeUtc { get; set; }

        public int RevertCount { get; set; }

        public int BurnoutScore { get; set; }
        public string BurnoutStatus { get; set; } = string.Empty;
    }
}
