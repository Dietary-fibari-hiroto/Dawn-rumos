using Microsoft.EntityFrameworkCore;
using rumos_server.Data;
using rumos_server.Features.Models;
using rumos_server.Features.Interface;
using rumos_server.Features.DTOs;

namespace rumos_server.Features.Repositories
{
    public class DeviceRepository:IDeviceRepository
    {
        private readonly AppDbContext _context;
        public DeviceRepository(AppDbContext context) {
            _context = context;
        }

        public async Task<IEnumerable<Device>> GetAllAsync() => await _context.Devices.ToListAsync();
        public async Task<Device?> GetByIdAsync(int id) => await _context.Devices.FindAsync(id);

        public async Task<string?> GetIpByIdAsync(int id) => await _context.Devices.Where(d => d.Id == id).Select(d => d.Ip_v4).FirstOrDefaultAsync();
        public async Task<string?> GetNameByIdAsync(int id)=> await _context.Devices.Where(d =>d.Id==id).Select(d => d.Name).FirstOrDefaultAsync();

        //デバイスをプラットフォームごとに一括で取ってくる関数
        public async Task<List<PlatformWithDevicesDto>> GetAllDevicesGroupedByPlatformAsync() {
            var result = await _context.Platforms
                .Include(p => p.Devices) // ← Entityのナビゲーション参照
                .Select(p => new PlatformWithDevicesDto
                {
                    PlatformId = p.Id,
                    PlatformName = p.Name,
                    Devices = p.Devices.Select(d=> new DeviceDto
                    {
                        Id = d.Id,
                        Name = d.Name,
                        Series = p.Id,        // Entity に無いので追加するならここ
                        IsPower = false,    // 初期値入れるならここ
                        R = 0,
                        G = 0,
                        B = 0,
                        Brightness = 0
                    }).ToList() // ← EntityのリストをそのままDTOに格納
                })
                .ToListAsync();

            return result;
        }

        public async Task<Device> AddAsync(Device device)
        {
            _context.Devices.Add(device);
            await _context.SaveChangesAsync();
            return device;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.Devices.FindAsync(id);
            if(entity == null)
            {
                return false;
            }
            _context.Devices.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
    public class PresetRepository:IPresetRepository
    {
        private readonly AppDbContext _context;
        public PresetRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Preset>> GetAllAsync() => await _context.Presets.ToListAsync();
        public async Task<Preset> AddAsync(Preset preset)
        {
            _context.Presets.Add(preset);
            await _context.SaveChangesAsync();
            return preset;
        }


        //一括発光のためのデータを取得するためのリポジトリ
        public async Task<List<Preset_device_map>> GetDeviceMapAsync(int id) => await _context.Preset_device_maps.Include(d=>d.Device).Where(d => d.Preset_id == id).ToListAsync();
    }

    public class RoomRepository : IRoomRepository {
        private readonly AppDbContext _context;
        public RoomRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Room>> GetAllAsync()=> await _context.Rooms.ToListAsync();
    }

}
