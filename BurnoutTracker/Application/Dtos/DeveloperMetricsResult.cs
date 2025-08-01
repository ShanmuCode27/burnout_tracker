namespace BurnoutTracker.Application.Dtos
{
    public class DeveloperMetricsResult
    {
        public int WeeklyCommitCount { get; set; }
        public int TotalCommitCount { get; set; }
        public int PullRequestCount { get; set; }
        public int ReviewChangesCount { get; set; }
        public int NightWorkCount { get; set; }
        public string? LatestWorkTimeUtc { get; set; }
        public int RevertCount { get; set; }
        public int BurnoutScore { get; set; }
        public string BurnoutStatus { get; set; } = string.Empty;
    }

}
