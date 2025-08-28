using rumos_server.Features.Models;

namespace rumos_server.Features.Interface;

public interface IDeviceRepository
{
    Task<IEnumerable<Device>> GetAllAsync();
    Task<Device?> GetByIdAsync(int id);

    Task<string?> GetIpByIdAsync(int id);
    Task<string?> GetNameByIdAsync(int id);
}

public interface IPlatformRepository
{
    Task<IEnumerable<Platform>> GetAllAsync();
}

