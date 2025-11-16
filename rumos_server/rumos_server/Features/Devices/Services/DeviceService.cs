using rumos_server.Features.DTOs;
using rumos_server.Features.Interface;
using rumos_server.Features.Models;
namespace rumos_server.Features.Services
{
    public class DeviceService : IDeviceService
    {
        private readonly IDeviceRepository _repo;

        public DeviceService(IDeviceRepository repo)
        {
            _repo = repo;
        }

        public Task<IEnumerable<Device>> GetDeviceAsync() => _repo.GetAllAsync();
        public Task<Device?> GetDeviceAsync(int id) => _repo.GetByIdAsync(id);

        public Task<string?> GetTpIpAsync(int id) => _repo.GetIpByIdAsync(id);

        public Task<string?> GetDeviceNameAsync(int id) => _repo.GetNameByIdAsync(id);

        public Task<List<PlatformWithDevicesDto>> GetAllDevicesWithPlatformAsync() => _repo.GetAllDevicesGroupedByPlatformAsync();

        public async Task<Device> CreateDeviceAsync(CreateDeviceDto device) {
                        var addDevice = new Device()
                        {
                            Name = device.Name,
                            Ip_v4 = device.Ip_v4,
                            Platform_id = device.Platform_id,
                            Room_id = device.Room_id
                        };
            return await _repo.AddAsync(addDevice);
        }

        public async Task<bool> DeleteDeviceAsync(int id) => await _repo.DeleteAsync(id);
    }

    public class PresetService : IPresetService
    {
        private readonly IPresetRepository _repo;
        public PresetService(IPresetRepository repo)
        {
            _repo = repo;
        }
        public Task<IEnumerable<Preset>> GetAllPresetAsync() => _repo.GetAllAsync();
        public Task<Preset> CreateAsync(Preset preset) => _repo.AddAsync(preset);
        public Task<List<Preset_device_map>> GetMapsByIdAsync(int id) => _repo.GetDeviceMapAsync(id);
    }


    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _repo;
        public RoomService(IRoomRepository repo)
        {
            _repo = repo;
        }
        public async Task<IEnumerable<Room>> GetAllRoomAsync() => await _repo.GetAllAsync();
    }
}
