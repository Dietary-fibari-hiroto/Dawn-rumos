using rumos_server.Features.Models;

namespace rumos_server.Features.Logs
{
    public interface ILogService
    {
        // 簡単にログを残すためのメソッド
        Task LogAsync(string message, string category = "general", Models.LogLevel level = Models.LogLevel.Info, string? deviceId = null, Dictionary<string, object>? metadata = null);

        Task LogDebugAsync(string message, string category = "general", string? deviceId = null);
        Task LogInfoAsync(string message, string category = "general", string? deviceId = null, Dictionary<string, object>? metadata = null);
        Task LogWarningAsync(string message, string category = "general", string? deviceId = null);
        Task LogErrorAsync(string message, string category = "general", string? deviceId = null, Exception? exception = null);

        // 取得系
        Task<IEnumerable<LogEntry>> GetLogsAsync(int limit = 100);
        Task<IEnumerable<LogEntry>> GetLogsByCategoryAsync(string category, int limit = 100);
        Task<IEnumerable<LogEntry>> GetLogsByDeviceAsync(string deviceId, int limit = 100);
    }

}
