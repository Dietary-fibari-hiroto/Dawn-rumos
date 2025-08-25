//https有効化コマンド:dotnet run --launch-profile "https"

using Devicecontrol;
using DotNetEnv;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rumos_server.Data;
using rumos_server.Extensions;
using rumos_server.Externals.GrpcClients;
var builder = WebApplication.CreateBuilder(args);

//gRPCの接続設定とインスタンスを呼び出して接続
//GrpcChannel channel = GrpcChannel.ForAddress("http://localhost:50052");
var channel = GrpcChannel.ForAddress("http://localhost:50052", new GrpcChannelOptions
{
    LoggerFactory = LoggerFactory.Create(builder =>
    {
        builder.AddConsole().SetMinimumLevel(LogLevel.Debug);
    })
});

DawnDeviceControl.DawnDeviceControlClient client = new DawnDeviceControl.DawnDeviceControlClient(channel);
//builder.Services.AddSingleton(new DawnDeviceControlClient(channel));//DI登録
builder.Services.AddSingleton(new GrpcService(client));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//DI�o�^
builder.Services.AddDbContext<AppDbContext>(options => options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"), new MySqlServerVersion(new Version(8, 0, 38))));
builder.Services.RegisterRepositories();
builder.Services.RegisterServices();

var app = builder.Build();
app.MapGet("/", () => "Dawn-Rumos");

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
