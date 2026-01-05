//https有効化コマンド:dotnet run --launch-profile "https"
using Devicecontrol;
using DotNetEnv;
using Grpc.Net.Client;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using rumos_server.Configs.Extensions;
using rumos_server.Data;
using rumos_server.Externals.GrpcClients;
using rumos_server.Externals.MqttClients;
using rumos_server.Features;



var builder = WebApplication.CreateBuilder(args);


//env読み込み
Env.Load();


builder.Services.AddMqttServices();
builder.Services.AddGrpcClients();
builder.Services.AddDatabaseContext(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.RegisterRepositories();
builder.Services.RegisterServices();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// パイプライン設定
app.ConfigurePipeline()
   .MapEndpoints();


app.Run();


/**
 * API-KEY
 * 
 */

/*
//開発環境で外部から接続したいときは以下をappsetting.jsonに追加
  "Kestrel": {
    "Endpoints": {
      "Http": {
        "Url": "http://0.0.0.0:7032"
      }
    }
  }
*/