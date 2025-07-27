using BurnoutTracker.Application.Services;
using BurnoutTracker.Domain.Extensions;
using BurnoutTracker.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BurnoutTracker.Controllers
{
    [ApiController]
    [Route("api/analytics")]
    public class DeveloperAnalyticsController : ControllerBase
    {
        private readonly IBurnoutDetectionService _burnoutDetectionService;
        private readonly BTDbContext _db;

        public DeveloperAnalyticsController(BTDbContext db, IBurnoutDetectionService burnoutDetectionService)
        {
            _db = db;
            _burnoutDetectionService = burnoutDetectionService;
        }

        [HttpGet("{connectionId}")]
        public async Task<IActionResult> GetLatestStates(long connectionId)
        {
            var latestStates = await _burnoutDetectionService.GetLatestDevState(connectionId);

            return Ok(latestStates);
        }

        [HttpGet("{connectionId}/history")]
        public async Task<IActionResult> GetHistory(long connectionId)
        {
            var history = await _burnoutDetectionService.GetDevHistory(connectionId);

            return Ok(history);
        }

        [HttpGet("{connectionId}/commits")]
        public async Task<IActionResult> GetCommitStats(long connectionId, [FromQuery] int days)
        {
            var stats = await _burnoutDetectionService.GetCommitStats(connectionId, days);

            return Ok(stats);
        }

        [HttpGet("{connectionId}/developers")]
        public async Task<IActionResult> GetDeveloperStats(long repoId, long connectionId)
        {
            var userId = Request.GetUserIdFromJwtToken();
            var stats = await _burnoutDetectionService.GetDeveloperStatsAsync(connectionId, userId ?? 0);

            return Ok(stats);
        }

        [HttpGet("{connectionId}/developers/{login}")]
        public async Task<IActionResult> GetDeveloperDetails(long connectionId, string login)
        {
            var userId = Request.GetUserIdFromJwtToken();
            if (userId == null) return Unauthorized();

            var details = await _burnoutDetectionService.GetDeveloperDetailAsync(connectionId, userId.Value, login);
            if (details == null) return NotFound();

            return Ok(details);
        }
    }
}
