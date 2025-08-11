using Grpc.PowerService;
using Grpc.Net.Client;
using Myservice;

GrpcChannel channel = GrpcChannel.ForAddress("http://localhost:50051");
MyService.MyServiceClient client = new MyService.MyServiceClient(channel);


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
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
app.MapGet("/power",()=>{
    Console.WriteLine("ASP.NET /power受信");
    ps.ChangeDevicePower(client);
});

app.Run();
