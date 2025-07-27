using BurnoutTracker.Application.Services;
using BurnoutTracker.Infrastructure;
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
    }
}
