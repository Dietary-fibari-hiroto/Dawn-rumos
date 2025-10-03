using rumos_server.Features.Models;
using rumos_server.Features.Interface;
using rumos_server.Features.Repositories;
namespace rumos_server.Features.Services
{
    public class DeviceService:IDeviceService
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
}
