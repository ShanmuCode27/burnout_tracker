namespace BurnoutTracker.Application.Dtos
{
    public class DeveloperActivityDto
    {
        public string DeveloperLogin { get; set; } = string.Empty;
        public int WeeklyCommitCount { get; set; }
    }
}
