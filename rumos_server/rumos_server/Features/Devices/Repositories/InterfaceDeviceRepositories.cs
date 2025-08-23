using rumos_server.Features.Models;

namespace rumos_server.Features.Interface;

public interface IDeviceRepository
{
    Task<IEnumerable<Device>> GetAllAsync();
    Task<Device?> GetByIdAsync(int id);
}

public interface IPlatformRepository
{
    Task<IEnumerable<Platform>> GetAllAsync();
}

