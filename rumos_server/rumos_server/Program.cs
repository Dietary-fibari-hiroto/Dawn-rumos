//https有効化コマンド:dotnet run --launch-profile "https"
using Devicecontrol;
using DotNetEnv;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.OpenApi.Models;
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
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Dawn Rumos API",
        Version = "v1",
        Description = "IoT照明制御システムのAPI"
    });

    // API Keyのセキュリティ定義を追加
    options.AddSecurityDefinition("X-API-Key", new OpenApiSecurityScheme
    {
        Name = "X-API-Key",
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Description = "APIキーをヘッダーに入力してください",
        Scheme = "ApiKeyScheme"
    });

    // すべてのエンドポイントにセキュリティ要件を適用
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "X-API-Key"
                }
            },
            new string[] { }
        }
    });
});
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
