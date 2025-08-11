using Grpc.Net.Client;
using Myservice;

namespace Grpc.PowerService{
    public class PowerService {
        public async void ChangeDevicePower(MyService.MyServiceClient client){
            var reply = await client.GetDataAsync(new DataRequest { Query = "Yuzuki" });
            Console.WriteLine($"Value1: {reply.Value1}, Value2: {reply.Value2}");
        }
    }
}