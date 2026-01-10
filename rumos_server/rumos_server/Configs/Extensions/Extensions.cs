using Devicecontrol;
using Grpc.Net.Client;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using rumos_server.Data;
using rumos_server.Externals.GrpcClients;
using rumos_server.Externals.MqttClients;
using rumos_server.Features.Interface;
using rumos_server.Features.Repositories;
using rumos_server.Features.Services;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace rumos_server.Configs.Extensions;

public static class ServiceExtensions
{
    public static void RegisterRepositories(this IServiceCollection services)
    {
        services.AddScoped<IPlatformRepository, PlatformRepository>();
        services.AddScoped<IDeviceRepository,DeviceRepository>();
        services.AddScoped<IPresetRepository, PresetRepository>();
        services.AddScoped<IRoomRepository,RoomRepository>();
    }

    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddScoped<IPlatformService,PlatformService>();
        services.AddScoped<IDeviceService, DeviceService>();
        services.AddScoped<IPresetService, PresetService>();
        services.AddScoped<IRoomService,RoomService>();
    }

    public static IServiceCollection AddMqttServices(this IServiceCollection services)
    {
        // 1. 設定を環境変数から読み込み
        services.Configure<MqttSettings>(o =>
        {
            o.Broker = Environment.GetEnvironmentVariable("MQTTBROKER_IP") ?? "127.0.0.1";
            o.Port = int.TryParse(Environment.GetEnvironmentVariable("MQTTBROKER_PORT"), out var p) ? p : 1883;
            o.Username = Environment.GetEnvironmentVariable("MQTT_USERNAME");
            o.Password = Environment.GetEnvironmentVariable("MQTT_PASSWORD");
            o.UseTls = bool.TryParse(Environment.GetEnvironmentVariable("MQTT_USE_TLS"), out var tls) && tls;
            o.CleanSession = false; // 永続セッション推奨
            o.KeepAliveSeconds = 60;
        });

        // 2. MqttConnectionService を Singleton として登録
        // → アプリ全体で1つの接続を共有
        services.AddSingleton<MqttConnectionService>();

        // 3. MqttHostedService を BackgroundService として登録
        // → アプリ起動時に自動で接続
        services.AddHostedService<MqttHostedService>();

        // 4. MqttService を Scoped として登録
        // → リクエストごとに新しいインスタンス
        services.AddScoped<MqttService>();

        // 5. ヘルスチェックを登録
        services.AddHealthChecks()
            .AddCheck<MqttHealthCheck>(
                "mqtt",
                failureStatus: HealthStatus.Unhealthy,
                tags: new[] { "mqtt", "ready" });

        // Dos対策（Rate Limiting）
        services.AddRateLimiter(options =>
        {
            options.AddFixedWindowLimiter("api", opt =>
            {
                opt.Window = TimeSpan.FromMinutes(1);
                opt.PermitLimit = 100;
            });
        });

        // CORS設定
        services.AddCors(options =>
        {
            options.AddPolicy("MauiApp", policy =>
            {
                policy.WithOrigins("https://localhost:7001")
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        return services;
    }
    public static void MapHealthCheckEndpoints(this WebApplication app)
    {
        // 基本のヘルスチェック
        app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";

                var result = System.Text.Json.JsonSerializer.Serialize(new
                {
                    status = report.Status.ToString(),
                    checks = report.Entries.Select(e => new
                    {
                        name = e.Key,
                        status = e.Value.Status.ToString(),
                        description = e.Value.Description,
                        data = e.Value.Data
                    }),
                    totalDuration = report.TotalDuration
                });

                await context.Response.WriteAsync(result);
            }
        });

        // Readiness チェック（K8s/Container Apps用）
        app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("ready")
        });

        // Liveness チェック（K8s/Container Apps用）
        app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            Predicate = _ => false // 基本的なアプリ起動確認のみ
        });
    }

    public static IServiceCollection AddGrpcClients(this IServiceCollection services)
    {
        //gRPCの接続設定とインスタンスを呼び出して接続
        //GrpcChannel channel = GrpcChannel.ForAddress("http://localhost:50052");
        var channel = GrpcChannel.ForAddress("http://localhost:50052", new GrpcChannelOptions
        {
            LoggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole().SetMinimumLevel(LogLevel.Debug);
            })
        });

        DawnDeviceControl.DawnDeviceControlClient client = new DawnDeviceControl.DawnDeviceControlClient(channel);//DI登録
        services.AddSingleton(new GrpcService(client));

        return services;
    }

    public static IServiceCollection AddDatabaseContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                Environment.GetEnvironmentVariable("DEFAULTCONNECTION")
            ));
        return services;
    }
}

public class AddHeaderOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters ??= new List<OpenApiParameter>();

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "X-API-Key",
            In = ParameterLocation.Header,
            Required = false
        });
    }
}
