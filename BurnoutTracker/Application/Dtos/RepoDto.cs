namespace BurnoutTracker.Application.Dtos
{
    public class RepoDto
    {
        public string Name { get; set; } = "";
        public string FullName { get; set; } = "";
        public OwnerDto Owner { get; set; } = new();
    }
}
