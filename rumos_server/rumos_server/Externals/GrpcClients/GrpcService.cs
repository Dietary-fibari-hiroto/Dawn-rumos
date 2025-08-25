using Grpc.Net.Client;
using Devicecontrol;
using DotNetEnv;
namespace rumos_server.Externals.GrpcClients
{
    public class GrpcService
    {
        private readonly DawnDeviceControl.DawnDeviceControlClient _client;

        public GrpcService(DawnDeviceControl.DawnDeviceControlClient client)
        {
            _client = client;
        }

        public async Task<RSetPowerReply> PowerSupply(string ip)
        {
            var req = new SetDeviceRequest
            {
                Ip = ip
            };
            try
            {
                var res = await _client.SetPowerChangeAsync(req);
                return new RSetPowerReply(res.Success, res.Message, res.IsOn);
            }
            catch(Exception ex)
            {
                return new RSetPowerReply(false, $"Unexpected Error: {ex.Message}", false);
            }
            
        }

        public async Task<RGetStatusReply> GetDevicePower(string ip)
        {
            var req = new SetDeviceRequest
            {
                Ip = ip
            };
            try
            {
                var res = await _client.GetDevicePowerAsync(req);
                return new RGetStatusReply(res.IsConnect, res.IsOn);
            }catch(Exception ex)
            {
                return new RGetStatusReply(false, false);
            }
        }
    }
}
