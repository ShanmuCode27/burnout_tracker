using BurnoutTracker.Application.Dtos.Github;
using BurnoutTracker.Application.Dtos;

namespace BurnoutTracker.Utilities
{
    public class DeveloperMetricsCalculator
    {
        public List<DeveloperActivityDto> Calculate(
            List<GitHubCommitDto> commits,
            List<GitHubPullRequestDto> pullRequests)
        {
            var developers = commits
                .Where(c => c.Author?.Login != null)
                .GroupBy(c => c.Author!.Login)
                .ToDictionary(g => g.Key, g => g.ToList());

            var result = new List<DeveloperActivityDto>();

            foreach (var dev in developers)
            {
                var login = dev.Key;
                var devCommits = dev.Value;

                var weeklyCommitCount = devCommits.Count / 4;
                var totalCommitCount = devCommits.Count;
                var revertCount = devCommits.Count(c => c.Commit.Message.Contains("revert", StringComparison.OrdinalIgnoreCase));
                var nightCommits = devCommits
                    .Where(c => c.Commit.Author.Date.Hour is >= 0 and <= 5)
                    .ToList();

                var latestWorkTime = nightCommits
                    .OrderByDescending(c => c.Commit.Author.Date)
                    .FirstOrDefault()?.Commit.Author.Date.ToString("HH:mm");

                var reviewChanges = pullRequests
                    .Where(pr => pr.User.Login == login)
                    .Sum(pr => pr.ReviewComments);

                var prCount = pullRequests.Count(pr => pr.User.Login == login);

                var score = 0;
                score += totalCommitCount > 25 ? 10 : 0;
                score += revertCount > 3 ? 15 : 0;
                score += reviewChanges > 10 ? 10 : 0;
                score += nightCommits.Count > 2 ? 10 : 0;
                score += weeklyCommitCount == 0 ? 30 : 0;

                var status = score switch
                {
                    <= 30 => "Active",
                    <= 60 => "Warning",
                    _ => "BurnedOut"
                };

                result.Add(new DeveloperActivityDto
                {
                    DeveloperLogin = login,
                    WeeklyCommitCount = weeklyCommitCount,
                    TotalCommitCount = totalCommitCount,
                    PullRequestCount = prCount,
                    ReviewChangesCount = reviewChanges,
                    NightWorkCount = nightCommits.Count,
                    LatestWorkTimeUtc = latestWorkTime,
                    BurnoutScore = score,
                    BurnoutStatus = status
                });
            }

            return result;
        }

        public DeveloperMetricsResult Calculate(List<GitHubCommitDto> commits, List<GitHubPullRequestDto> pullRequests, string login)
        {
            var devCommits = commits.Where(c => c.Author?.Login == login).ToList();

            var weeklyCommitCount = devCommits.Count / 4;
            var totalCommitCount = devCommits.Count;
            var revertCount = devCommits.Count(c => c.Commit.Message.Contains("revert", StringComparison.OrdinalIgnoreCase));
            var nightCommits = devCommits
                .Where(c => c.Commit.Author.Date.Hour >= 0 && c.Commit.Author.Date.Hour <= 5)
                .ToList();

            var latestWorkTime = nightCommits
                .OrderByDescending(c => c.Commit.Author.Date)
                .FirstOrDefault()?.Commit.Author.Date.ToString("HH:mm");

            var reviewChanges = pullRequests
                .Where(pr => pr.User.Login == login)
                .Sum(pr => pr.ReviewComments);

            var prCount = pullRequests.Count(pr => pr.User.Login == login);

            var score = 0;
            score += totalCommitCount > 25 ? 10 : 0;
            score += revertCount > 3 ? 15 : 0;
            score += reviewChanges > 10 ? 10 : 0;
            score += nightCommits.Count > 2 ? 10 : 0;
            score += weeklyCommitCount == 0 ? 30 : 0;

            var status = score switch
            {
                <= 30 => "Active",
                <= 60 => "Warning",
                _ => "BurnedOut"
            };

            return new DeveloperMetricsResult
            {
                WeeklyCommitCount = weeklyCommitCount,
                TotalCommitCount = totalCommitCount,
                PullRequestCount = prCount,
                ReviewChangesCount = reviewChanges,
                NightWorkCount = nightCommits.Count,
                LatestWorkTimeUtc = latestWorkTime,
                RevertCount = revertCount,
                BurnoutScore = score,
                BurnoutStatus = status
            };
        }

    }

}
