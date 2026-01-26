using Microsoft.EntityFrameworkCore;
using rumos_server.Features.Models;
namespace rumos_server.Data;

public class CosmosDbContext : DbContext
{
    public CosmosDbContext(DbContextOptions<CosmosDbContext> options) : base(options)
    {
    }

    public DbSet<LogEntry> Logs => Set<LogEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Logsコンテナの設定
        modelBuilder.Entity<LogEntry>(entity =>
        {
            // コンテナ名
            entity.ToContainer("Logs");

            // パーティションキー
            entity.HasPartitionKey(e => e.Category);

            // プロパティ設定
            entity.Property(e => e.Id).ToJsonProperty("id");
            entity.Property(e => e.Category).ToJsonProperty("category");
            entity.Property(e => e.Level).ToJsonProperty("level");
            entity.Property(e => e.Message).ToJsonProperty("message");
            entity.Property(e => e.DeviceId).ToJsonProperty("deviceId");
            entity.Property(e => e.Timestamp).ToJsonProperty("timestamp");
            entity.Property(e => e.Ttl).ToJsonProperty("ttl");

            // Metadataは所有エンティティとして扱わず、コンバーターで処理
            entity.Property(e => e.Metadata)
                .ToJsonProperty("metadata")
                .HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                    v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(v, (System.Text.Json.JsonSerializerOptions?)null)
                );
        });
    }
}