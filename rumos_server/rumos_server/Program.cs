//https有効化コマンド:dotnet run --launch-profile "https"
using Devicecontrol;
using DotNetEnv;
using Grpc.Net.Client;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using rumos_server.Data;
using rumos_server.Extensions;
using rumos_server.Externals.GrpcClients;
using rumos_server.Externals.MqttClients;
using rumos_server.SignalR.Hubs;



var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR();

//env読み込み
DotNetEnv.Env.Load();

//MQTT設定
builder.Services.Configure<MqttSettings>(o =>
{
    o.Broker = Environment.GetEnvironmentVariable("MQTTBROKER_IP") ?? "127.0.0.1";
    o.Port = int.TryParse(Environment.GetEnvironmentVariable("MQTTBROKER_PORT"), out var p) ? p : 1883;
    o.Username = Environment.GetEnvironmentVariable("MQTT_USERNAME");
    o.Password = Environment.GetEnvironmentVariable("MQTT_PASSWORD");
    o.UseTls = bool.TryParse(Environment.GetEnvironmentVariable("MQTT_USE_TLS"), out var tls) && tls;
    o.CleanSession = false; // 永続セッション推奨
    o.KeepAliveSeconds = 60;
});

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
builder.Services.AddSingleton(new GrpcService(client));
builder.Services.AddSingleton<MqttConnectionService>();

builder.Services.AddHostedService<MqttHostedService>();
builder.Services.AddScoped<MqttService>();



builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//DI�o�^
builder.Services.AddDbContext<AppDbContext>(options => options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"), new MySqlServerVersion(new Version(8, 0, 38))));

builder.Services.RegisterRepositories();
builder.Services.RegisterServices();

var app = builder.Build();
app.MapGet("/", () => "Dawn-Rumos");
app.MapHub<LedHub>("/ledstate");
app.MapHub<TestHub>("/test");

app.MapGet("/api/test-broadcast", async (IHubContext<TestHub> hub) =>
{
    var msg = $"サーバーからのブロードキャスト！ {DateTime.Now}";
    await hub.Clients.All.SendAsync("ReceiveMessage", "Server", msg);
    Console.WriteLine($"🧪 {msg}");
    return Results.Ok("送信完了");
});


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();



app.Run();
