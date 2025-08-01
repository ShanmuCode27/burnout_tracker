namespace BurnoutTracker.Utilities
{
    public static class StringHelper
    {
        public static (string owner, string repo) ParseRepoUrl(string url)
        {
            // Parse https://github.com/{owner}/{repo} for example
            var parts = new Uri(url).AbsolutePath.Trim('/').Split('/');
            return (parts[0], parts[1]);
        }
    }
}
