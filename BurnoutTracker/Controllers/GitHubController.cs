using BurnoutTracker.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BurnoutTracker.Controllers
{
    [Route("api/github")]
    [ApiController]
    public class GitHubController : ControllerBase
    {
        private readonly IGitHubService _githubService;

        public GitHubController(IGitHubService githubService)
        {
            _githubService = githubService;
        }

        [HttpPost("token")]
        public async Task<IActionResult> SaveToken([FromBody] string token)
        {
            var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await _githubService.SaveUserTokenAsync(userId, token);
            return Ok();
        }

        [HttpGet("repos")]
        public async Task<IActionResult> GetRepos()
        {
            var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var repos = await _githubService.GetUserReposAsync(userId);
            return Ok(repos);
        }

    }
}
