using rumos_server.Features.Models;

namespace rumos_server.Features.Interface;

public interface IDeviceRepository
{
    Task<IEnumerable<Device>> GetAllAsync();
    Task<Device?> GetByIdAsync(int id);

    Task<string?> GetIpByIdAsync(int id);
    Task<string?> GetNameByIdAsync(int id);

    //登録処理めちゃくちゃ忘れてた。
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


