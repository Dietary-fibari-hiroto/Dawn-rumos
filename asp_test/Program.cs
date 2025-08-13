using Grpc.PowerService;
using Grpc.Net.Client;
using Myservice;
using Devicecontrol;
using DotNetEnv;



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


// Add services to the container.-
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


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
app.MapGet("/sp", () =>
{
    Console.WriteLine("ASP.NET /📡ps発信");
    ps.SpecificChangeDevicePower(sendClient);
});
app.MapGet("/sp2", () =>
{
    Console.WriteLine("ASP.NET /📡ps2発信");
    ps.Sp2(sendClient);
});
app.MapGet("/sp3", () =>
{
    Console.WriteLine("ASP.NET /📡ps3発信");
    ps.Sp3(sendClient);
});


app.Run();
