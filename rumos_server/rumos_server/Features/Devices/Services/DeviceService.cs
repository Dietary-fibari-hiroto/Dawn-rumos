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


    //MagicRoutine
    public class PresetService : IPresetService
    {
        private readonly IPresetRepository _repo;
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _http;
        public PresetService(IPresetRepository repo, IWebHostEnvironment env, IHttpContextAccessor http)
        {
            _repo = repo;
            _env = env;
            _http = http;
        }
        public Task<IEnumerable<Preset>> GetAllPresetAsync() => _repo.GetAllAsync();
        public async Task<Preset> CreateAsync(PresetCreateDto request)
        {
            var name = request.Name;

            var uploadFolder = Path.Combine(_env.WebRootPath, "images");
            if (!Directory.Exists(uploadFolder)) Directory.CreateDirectory(uploadFolder);

            //ファイル名をユニークに
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(request.File.FileName)}";
            var filePath = Path.Combine(uploadFolder, fileName);

            using(var stream = new FileStream(filePath, FileMode.Create))
            {
                await request.File.CopyToAsync(stream);
            }

            var Request = _http.HttpContext?.Request!;

            var fileUrl = $"{Request.Scheme}://{Request.Host}/images/{fileName}";



            Preset data = new()
            {
                Name = name,
                Img_url = fileUrl,
            };
            return await _repo.AddAsync(data);

        }
        public async Task<bool> DeletePresetAsync(int id)=> await _repo.DeleteAsync(id);
        public Task<List<Preset_device_map>> GetMapsByIdAsync(int id) => _repo.GetDeviceMapAsync(id);

        public async Task<bool> RegisterDeviceMapAsync(List<DeviceDto> list, int id)
        {
            List<Preset_device_map> devices = new();
            foreach(DeviceDto device in list)
            {
                if (device == null) continue;
                Preset_device_map conversion = new()
                {
                    Preset_id=id,
                    Device_id= device.Id,
                    R = device.R,
                    G = device.G,
                    B = device.B,
                    Brightness = device.Brightness,
                    Mode_id = 1
                };
                
                devices.Add(conversion);

            }

            return await _repo.PostDeviceMapAsync(devices);
        }
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
