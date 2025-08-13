using Grpc.Net.Client;
using Myservice;
using Devicecontrol;
using DotNetEnv;



namespace Grpc.PowerService
{
    public class PowerService
    {

        public async void ChangeDevicePower(MyService.MyServiceClient client)
        {
        
            var reply = await client.GetDataAsync(new DataRequest { Query = "Yuzuki" });
            Console.WriteLine($"Value1: {reply.Value1}, Value2: {reply.Value2}");
        }

        public async void SpecificChangeDevicePower(DeviceControl.DeviceControlClient client)
        {

            var req = new SetPowerRequest
            {
                DeviceId = "plug-01",
                Ip = Environment.GetEnvironmentVariable("DEVICE_IP"),
                AccountEmail = Environment.GetEnvironmentVariable("DEVICE_EMAIL"),
                AccountPassword = Environment.GetEnvironmentVariable("DEVICE_PASSWORD")
            };

            var res = await client.SetPowerAsync(req);
            Console.WriteLine($"success={res.Success} message={res.Message}");
        }
        public async void Sp2(DeviceControl.DeviceControlClient client)
        {

            var req = new SetPowerRequest
            {
                DeviceId = "plug-02",
                Ip = Environment.GetEnvironmentVariable("DEVICE_IP2"),
               AccountEmail = Environment.GetEnvironmentVariable("DEVICE_EMAIL"),
                AccountPassword = Environment.GetEnvironmentVariable("DEVICE_PASSWORD")
            };

            var res = await client.SetPowerAsync(req);
            Console.WriteLine($"success={res.Success} message={res.Message}");
        }

        public async void Sp3(DeviceControl.DeviceControlClient client)
        {

            var req = new SetPowerRequest
            {
                DeviceId = "plug-03",
                Ip = Environment.GetEnvironmentVariable("DEVICE_IP3"),
                AccountEmail = Environment.GetEnvironmentVariable("DEVICE_EMAIL"),
                AccountPassword = Environment.GetEnvironmentVariable("DEVICE_PASSWORD")
            };

            var res = await client.SetPowerAsync(req);
            Console.WriteLine($"success={res.Success} message={res.Message}");
        }
    }
}