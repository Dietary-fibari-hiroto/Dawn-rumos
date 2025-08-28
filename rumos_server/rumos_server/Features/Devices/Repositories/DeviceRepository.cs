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
}
