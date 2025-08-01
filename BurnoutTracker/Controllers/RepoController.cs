using BurnoutTracker.Application.Dtos;
using BurnoutTracker.Application.Services;
using BurnoutTracker.Domain.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BurnoutTracker.Controllers
{
    [ApiController]
    [Route("api/repos")]
    //    [Authorize]
    public class ReposController : ControllerBase
    {
        private readonly IRepoService _repoService;

        public ReposController(IRepoService repoService)
        {
            _repoService = repoService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var userId = Request.GetUserIdFromJwtToken();
            if (userId == null)
                return Unauthorized();
            var repos = await _repoService.GetConnectedRepositoriesAsync(userId ?? 0);
            return Ok(repos);
        }

        [HttpPost("connect")]
        public async Task<IActionResult> Connect(ConnectRepoRequestDto request)
        {
            var userId = Request.GetUserIdFromJwtToken();
            if (userId == null)
                return Unauthorized();
            await _repoService.ConnectRepositoryAsync(userId ?? 0, request);
            return Ok(new { message = "Repository connected." });
        }

        [HttpDelete("{connectionId}")]
        public async Task<IActionResult> DeleteRepository(long connectionId)
        {
            var userId = Request.GetUserIdFromJwtToken();
            await _repoService.DeleteRepository(connectionId, userId ?? 0);

            return NoContent();
        }
    }
}
