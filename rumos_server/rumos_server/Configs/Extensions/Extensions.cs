using Devicecontrol;
using Grpc.Net.Client;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
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
        services.AddSingleton<MqttConnectionService>();
        services.AddHostedService<MqttHostedService>();
        services.AddScoped<MqttService>();

        //Dos対策
        services.AddRateLimiter(options =>
        {
            options.AddFixedWindowLimiter("api", opt =>
            {
                opt.Window = TimeSpan.FromMinutes(1);
                opt.PermitLimit = 100;
            });
        });

        services.AddCors(options =>
        {
            options.AddPolicy("MauiApp", policy =>
            {
                policy.WithOrigins("https://localhost:7001")
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });


        services.AddSwaggerGen(c =>
        {
            c.OperationFilter<AddHeaderOperationFilter>();
        });


        return services;
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
