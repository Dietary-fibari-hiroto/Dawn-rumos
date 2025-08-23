using rumos_server.Features.Models;

namespace rumos_server.Features.Interface
{
public interface IPlatformService
    {
        Task<IEnumerable<Platform>> GetPlatformAsync();
    }
    public interface IDeviceService
    {
        Task<IEnumerable<Device>> GetDeviceAsync();
        Task<Device?> GetDeviceAsync(int id);
    }
}
