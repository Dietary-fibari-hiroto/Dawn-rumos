namespace rumos_server.Features.Models
{
    public class LogEntry
    {
        /// <summary>
        /// 一意のID（CosmosDB必須）
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// ログのカテゴリ（パーティションキー）
        /// 例: "device", "api", "system", "mqtt"
        /// </summary>
        public string Category { get; set; } = "general";

        /// <summary>
        /// ログレベル
        /// </summary>
        public LogLevel Level { get; set; } = LogLevel.Info;

        /// <summary>
        /// ログメッセージ
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// 関連するデバイスID（あれば）
        /// </summary>
        public string? DeviceId { get; set; }

        /// <summary>
        /// 追加データ（JSON形式で何でも入れられる）
        /// </summary>
        public Dictionary<string, object>? Metadata { get; set; }

        /// <summary>
        /// ログ作成日時（UTC）
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// CosmosDBのTTL（秒）- nullで無期限
        /// </summary>
        public int Ttl { get; set; } = -1;
    }

    public enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Error,
        Critical
    }

}
