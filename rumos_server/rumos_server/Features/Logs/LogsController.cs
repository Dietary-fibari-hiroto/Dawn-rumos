using rumos_server.Features.Models;
using Microsoft.AspNetCore.Mvc;

namespace rumos_server.Features.Logs
{
    [ApiController]
    [Route("api/[controller]")]
    public class LogsController : ControllerBase
    {
        private readonly ILogService _logService;

        public LogsController(ILogService logService)
        {
            _logService = logService;
        }

  
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LogEntry>>> GetAll([FromQuery] int limit = 100)
        {
            var logs = await _logService.GetLogsAsync(limit);
            return Ok(logs);
        }

        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<LogEntry>>> GetByCategory(
            string category,
            [FromQuery] int limit = 100)
        {
            var logs = await _logService.GetLogsByCategoryAsync(category, limit);
            return Ok(logs);
        }


        [HttpGet("device/{deviceId}")]
        public async Task<ActionResult<IEnumerable<LogEntry>>> GetByDevice(
            string deviceId,
            [FromQuery] int limit = 100)
        {
            var logs = await _logService.GetLogsByDeviceAsync(deviceId, limit);
            return Ok(logs);
        }

        [HttpPost]
        public async Task<ActionResult<LogEntry>> Create([FromBody] CreateLogRequest request)
        {
            await _logService.LogAsync(
                request.Message,
                request.Category ?? "general",
                request.Level ?? Models.LogLevel.Info,
                request.DeviceId,
                request.Metadata
            );

            return Ok(new { success = true, message = "Log created" });
        }
    }

    public class CreateLogRequest
    {
        public string Message { get; set; } = string.Empty;
        public string? Category { get; set; }
        public rumos_server.Features.Models.LogLevel? Level { get; set; }
        public string? DeviceId { get; set; }
        public Dictionary<string, object>? Metadata { get; set; }
    }
}
