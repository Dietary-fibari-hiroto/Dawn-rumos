using Microsoft.EntityFrameworkCore;
using rumos_server.Data;
using rumos_server.Features.Logs;

namespace rumos_server.Configs.Extensions
{

    public static class CosmosDbExtensions
    {
        /// <summary>
        /// CosmosDB (EF Core) のサービスを登録
        /// </summary>
        public static IServiceCollection AddCosmosDb(this IServiceCollection services)
        {
            var cosmosEndpoint = Environment.GetEnvironmentVariable("COSMOS_ENDPOINT")
                ?? "https://localhost:8081";
            var cosmosKey = Environment.GetEnvironmentVariable("COSMOS_KEY")
                ?? "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
            var databaseName = "RumosDb";

            services.AddDbContext<CosmosDbContext>(options =>
            {
                options.UseCosmos(
                    cosmosEndpoint,
                    cosmosKey,
                    databaseName,
                    cosmosOptions =>
                    {
                        //ローカルエミュレーターの場合のみSSL検証をスキップ
                        if (cosmosEndpoint.Contains("localhost"))
                        {
                            cosmosOptions.HttpClientFactory(() => new HttpClient(new HttpClientHandler
                            {
                                ServerCertificateCustomValidationCallback =
                                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                            }));
                            cosmosOptions.ConnectionMode(Microsoft.Azure.Cosmos.ConnectionMode.Gateway);
                        }
                    }
                );
            });

            // Repository & Service
            services.AddScoped<ILogRepository, LogRepository>();
            services.AddScoped<ILogService, LogService>();

            return services;
        }

        /// <summary>
        /// CosmosDBのデータベースとコンテナを初期化
        /// アプリ起動時に呼び出す
        /// </summary>
        public static async Task InitializeCosmosDbAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<CosmosDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<CosmosDbContext>>();

            try
            {
                logger.LogInformation("Initializing CosmosDB with EF Core...");

                // データベースとコンテナを作成（EF Coreが自動でやってくれる）
                await context.Database.EnsureCreatedAsync();

                logger.LogInformation("CosmosDB initialized successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to initialize CosmosDB");
                throw;
            }
        }
    }
}
