namespace BurnoutTracker.Dtos
{
    public class RepoDto
    {
        public string Name { get; set; } = "";
        public string Full_Name { get; set; } = "";
        public Owner Owner { get; set; } = new();
    }
}
