namespace BurnoutTracker.Application.Dtos
{
    public class CommitStatsDto
    {
        public string Developer { get; set; }
        public int TotalCommits { get; set; }
        public int AvgCommitsPerWeek { get; set; }
        public List<StateCountDto> States { get; set; } = new();
    }

    public class StateCountDto
    {
        public DateTime RecordedAt { get; set; }
        public string State { get; set; }
    }
}
