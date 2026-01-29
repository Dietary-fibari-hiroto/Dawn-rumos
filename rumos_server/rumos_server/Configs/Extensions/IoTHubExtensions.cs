using rumos_server.Externals.IoTHubClients;

namespace rumos_server.Configs.Extensions
{
    public static class IoTHubExtensions
    {
        public static IServiceCollection AddIoTHubServices(this IServiceCollection services)
        {
            // 設定バインド
            services.AddOptions<IoTHubSettings>()
                .BindConfiguration("IoTHub")
                .ValidateDataAnnotations();

            // Singleton: 接続管理
            services.AddSingleton<IoTHubConnectionService>();

            // Scoped: ビジネスロジック
            services.AddScoped<IoTHubService>();

            // HostedService: 起動時に接続
            services.AddHostedService<IoTHubHostedService>();

            // HealthCheck
            services.AddHealthChecks()
                .AddCheck<IoTHubHealthCheck>("iothub");

            return services;
        }
    }
}