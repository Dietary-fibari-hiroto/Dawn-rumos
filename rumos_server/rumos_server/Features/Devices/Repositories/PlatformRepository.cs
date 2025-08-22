using Microsoft.EntityFrameworkCore;
using rumos_server.Data;
using rumos_server.Features.Models;
using rumos_server.Features.Interface;

namespace rumos_server.Features.Repositories
{
    public class PlatformRepository: IPlatformRepository
    {
        private readonly AppDbContext _context;
        public PlatformRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Platform>> GetAllAsync() => await _context.Platforms.ToListAsync();
    }
}
