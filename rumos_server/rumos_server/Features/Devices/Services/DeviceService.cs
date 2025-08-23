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
    }
}
