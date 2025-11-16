using rumos_server.Features.Models;
using rumos_server.Features.DTOs;

namespace rumos_server.Features.Interface;

public interface IDeviceRepository
{
    Task<IEnumerable<Device>> GetAllAsync();
    Task<Device?> GetByIdAsync(int id);

    Task<string?> GetIpByIdAsync(int id);
    Task<string?> GetNameByIdAsync(int id);

    Task<List<PlatformWithDevicesDto>> GetAllDevicesGroupedByPlatformAsync();

    //登録処理めちゃくちゃ忘れてた。
    Task<Device> AddAsync(Device device);
}

public interface IPlatformRepository
{
    Task<IEnumerable<Platform>> GetAllAsync();
}

public interface IPresetRepository {
    Task<IEnumerable<Preset>> GetAllAsync();
    Task<Preset> AddAsync(Preset preest);

    Task<List<Preset_device_map>> GetDeviceMapAsync(int id);
    //登録処理後で追加

}


public interface IRoomRepository {
    Task<IEnumerable<Room>> GetAllAsync();
}