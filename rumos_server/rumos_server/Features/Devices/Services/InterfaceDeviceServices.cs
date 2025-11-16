using rumos_server.Features.Models;
using rumos_server.Features.DTOs;

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
        Task<string?> GetTpIpAsync(int id);
        Task<string?> GetDeviceNameAsync(int id);
        Task<List<PlatformWithDevicesDto>> GetAllDevicesWithPlatformAsync();

        Task<Device> CreateDeviceAsync(CreateDeviceDto device);
    }

    public interface IPresetService {
        Task<IEnumerable<Preset>> GetAllPresetAsync();
        Task<Preset> CreateAsync(Preset preset);

        Task<List<Preset_device_map>> GetMapsByIdAsync(int id);
    }
    public interface IRoomService
    {
        Task<IEnumerable<Room>> GetAllRoomAsync();
    }

}
