using rumos_server.Features.Models;

namespace rumos_server.Features.Logs
{
    public class LogService : ILogService
    {
        private readonly ILogRepository _logRepository;
        private readonly ILogger<LogService> _logger;

        public LogService(ILogRepository logRepository, ILogger<LogService> logger)
        {
            _logRepository = logRepository;
            _logger = logger;
        }

        public async Task LogAsync(
            string message,
            string category = "general",
            Models.LogLevel level = Models.LogLevel.Info,
            string? deviceId = null,
            Dictionary<string, object>? metadata = null)
        {
            var logEntry = new LogEntry
            {
                Message = message,
                Category = category,
                Level = level,
                DeviceId = deviceId,
                Metadata = metadata
            };

            try
            {
                await _logRepository.CreateAsync(logEntry);
            }
            catch (Exception ex)
            {
                // CosmosDBへの保存が失敗しても、アプリは止めない
                _logger.LogError(ex, "Failed to save log to CosmosDB: {Message}", message);
            }
        }

        public Task LogDebugAsync(string message, string category = "general", string? deviceId = null)
            => LogAsync(message, category, Models.LogLevel.Debug, deviceId);

        public Task LogInfoAsync(string message, string category = "general", string? deviceId = null, Dictionary<string, object>? metadata = null)
            => LogAsync(message, category, Models.LogLevel.Info, deviceId, metadata);

        public Task LogWarningAsync(string message, string category = "general", string? deviceId = null)
            => LogAsync(message, category, Models.LogLevel.Warning, deviceId);

        public Task LogErrorAsync(string message, string category = "general", string? deviceId = null, Exception? exception = null)
        {
            var metadata = exception != null
                ? new Dictionary<string, object>
                {
                    ["exceptionType"] = exception.GetType().Name,
                    ["stackTrace"] = exception.StackTrace ?? ""
                }
                : null;

            return LogAsync(message, category, Models.LogLevel.Error, deviceId, metadata);
        }

        public Task<IEnumerable<LogEntry>> GetLogsAsync(int limit = 100)
            => _logRepository.GetAllAsync(limit);

        public Task<IEnumerable<LogEntry>> GetLogsByCategoryAsync(string category, int limit = 100)
            => _logRepository.GetByCategoryAsync(category, limit);

        public Task<IEnumerable<LogEntry>> GetLogsByDeviceAsync(string deviceId, int limit = 100)
            => _logRepository.GetByDeviceIdAsync(deviceId, limit);
    }
}
