using Microsoft.EntityFrameworkCore;
using rumos_server.Data;
using rumos_server.Features.Models;
using rumos_server.Features.Interface;

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
}
