using Microsoft.EntityFrameworkCore;
using rumos_server.Features.Models;

namespace rumos_server.Data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

        public DbSet<Platform> Platforms => Set<Platform>();
        public DbSet<Room> Rooms => Set<Room>();
        public DbSet<Device> Devices => Set<Device>();
    }
}
