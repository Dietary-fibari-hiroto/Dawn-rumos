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
        public DbSet<Preset> Presets => Set<Preset>();
        public DbSet<Preset_device_map> Preset_device_maps => Set<Preset_device_map>(); 
        public DbSet<Mode> Modes => Set<Mode>();

        //複合キーの設定
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Preset_device_map>().HasKey(p => new { p.Preset_id, p.Device_id });
            base.OnModelCreating(modelBuilder);
        }
    }
}
