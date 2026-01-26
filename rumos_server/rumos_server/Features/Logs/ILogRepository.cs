using rumos_server.Features.Models;


namespace rumos_server.Features.Logs
{
    public interface ILogRepository
    {
        /// <summary>
        /// ログを保存
        /// </summary>
        Task<LogEntry> CreateAsync(LogEntry log);

        /// <summary>
        /// 全ログ取得（最新順、上限あり）
        /// </summary>
        Task<IEnumerable<LogEntry>> GetAllAsync(int limit = 100);

        /// <summary>
        /// カテゴリ別にログ取得
        /// </summary>
        Task<IEnumerable<LogEntry>> GetByCategoryAsync(string category, int limit = 100);

        /// <summary>
        /// デバイスID別にログ取得
        /// </summary>
        Task<IEnumerable<LogEntry>> GetByDeviceIdAsync(string deviceId, int limit = 100);

        /// <summary>
        /// 期間指定でログ取得
        /// </summary>
        Task<IEnumerable<LogEntry>> GetByDateRangeAsync(DateTime from, DateTime to, int limit = 100);
    }
}
