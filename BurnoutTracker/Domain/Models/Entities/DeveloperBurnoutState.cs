namespace BurnoutTracker.Domain.Models.Entities
{
    public class DeveloperBurnoutState
    {
        public long Id { get; set; }
        public long UserRepositoryConnectionId { get; set; }
        public UserRepositoryConnection UserRepositoryConnection { get; set; } = null!;
        public string DeveloperLogin { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty; // Values like "Active", "Warning", "BurnedOut"
        public int WeeklyCommitCount { get; set; }
        public int TotalCommitCount { get; set; }
        public DateTime RecordedAt { get; set; }
    }
}
