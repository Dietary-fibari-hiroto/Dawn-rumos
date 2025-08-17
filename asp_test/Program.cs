using Grpc.PowerService;
using Grpc.Net.Client;
using Myservice;
using Devicecontrol;
using DotNetEnv;
using Grpc.IDevice;
using MqTest;
using Microsoft.AspNetCore.Mvc;




GrpcChannel channel = GrpcChannel.ForAddress("http://localhost:50051");
MyService.MyServiceClient client = new MyService.MyServiceClient(channel);

GrpcChannel sendChannel = GrpcChannel.ForAddress("http://localhost:50052");
DeviceControl.DeviceControlClient sendClient = new DeviceControl.DeviceControlClient(sendChannel);

var builder = WebApplication.CreateBuilder(args);
Env.Load();

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5250); // HTTP
    options.ListenAnyIP(7054, listenOptions =>
    {
        listenOptions.UseHttps(); // HTTPS
    });
});
builder.Services.ConfigureHttpJsonOptions(options =>
{
    // キー名の大文字小文字を気にせず受け付ける
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
    // プロパティ名の変換をしない（PascalCaseのまま出す）
    options.SerializerOptions.PropertyNamingPolicy = null;
});


// Add services to the container.-
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSingleton<MQTT_Test>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};



PowerService ps = new PowerService();

app.MapGet("/",()=>"Hello World");
app.MapGet("/power", () =>
{
    Console.WriteLine("ASP.NET /power受信");
    ps.ChangeDevicePower(client);
});
app.MapPost("/sp", () =>
{
    Console.WriteLine("ASP.NET /📡ps発信");
    ps.SpecificChangeDevicePower(sendClient);
});
app.MapPost("/sp2",async () =>
{
    Console.WriteLine("ASP.NET /📡ps2発信");
    var res = await ps.Sp2(sendClient);
    return Results.Ok(res);
});
app.MapPost("/sp3", () =>
{
    Console.WriteLine("ASP.NET /📡ps3発信");
    ps.Sp3(sendClient);
});
// ESP32に色を送るエンドポイント
app.MapPost("/mq", async ([FromBody] LedColor color, [FromServices] MQTT_Test mqtt) =>
{
    await mqtt.SendLedColorAsync(color);
    return Results.Ok(new { message = "Color sent", color });
});

app.Run();
