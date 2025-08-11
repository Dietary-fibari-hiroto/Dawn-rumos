using Grpc.Net.Client;
using Myservice;

var channel = GrpcChannel.ForAddress("http://localhost:50051");
var client = new MyService.MyServiceClient(channel);

var reply = await client.GetDataAsync(new DataRequest { Query = "Yuzuki" });
Console.WriteLine($"Value1: {reply.Value1}, Value2: {reply.Value2}");
