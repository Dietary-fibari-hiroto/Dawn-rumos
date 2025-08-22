using rumos_server.Features.Models;

namespace rumos_server.Features.Interface;

public interface IDeviceRepository
{
    Task<IEnumerable<Device>> GetAllAsync();
}

public interface IPlatformRepository
{
    Task<IEnumerable<Platform>> GetAllAsync();
}

