using Microsoft.EntityFrameworkCore;
using rumos_server.Features.Models;

namespace rumos_server.Data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

        public DbSet<Platform> Platforms { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<Preset> Presets { get; set; }
        public DbSet<Mode> Modes { get; set; }
        public DbSet<Preset_device_map> Preset_device_maps { get; set; }

        //複合キーの設定
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Preset_device_map>().HasKey(p => new { p.Preset_id, p.Device_id });

            //テーブル名の明示的指定
            modelBuilder.Entity<Platform>().ToTable("platforms");
            modelBuilder.Entity<Room>().ToTable("rooms");
            modelBuilder.Entity<Device>().ToTable("devices");
            modelBuilder.Entity<Preset>().ToTable("presets");
            modelBuilder.Entity<Mode>().ToTable("modes");
            modelBuilder.Entity<Preset_device_map>().ToTable("preset_device_maps");

            /*
            modelBuilder.Entity<Mode>().HasData(
                new Mode { Id = 1, Name = "Normal" },
                new Mode { Id = 2, Name = "Blink" },
                new Mode { Id = 3, Name = "Fade" }
            );

            modelBuilder.Entity<Platform>().HasData(
                new Platform { Id = 1, Name = "TP-Link", Description = "電源" },
                new Platform { Id = 2, Name = "Astrolume_Sangria", Description = "空いたワインボトルに装着するタイプのライト" },
                new Platform { Id = 2, Name = "Astrolume_Startrail", Description = "線系の照明,間接照明" }
            );

            modelBuilder.Entity<Room>().HasData(
                new Room { Id = 1, Name = "Living Room", Description = "Main living area" },
                new Room { Id = 2, Name = "Bedroom", Description = "Master bedroom" },
                new Room { Id = 3, Name = "Kitchen", Description = "Kitchen area" }
            );
            */

            // DeviceのIP制約など
            modelBuilder.Entity<Device>()
                .Property(d => d.Ip_v4)
                .HasMaxLength(15)
                .IsRequired(false);

            // Presetのデフォルト値設定
            modelBuilder.Entity<Preset>()
                .Property(p => p.Img_url)
                .HasDefaultValue(string.Empty);

            // RGB値の範囲制約（チェック制約）
            modelBuilder.Entity<Preset_device_map>()
                .ToTable(t => t.HasCheckConstraint("CK_Preset_device_map_R", "[R] >= 0 AND [R] <= 255"));

            modelBuilder.Entity<Preset_device_map>()
                .ToTable(t => t.HasCheckConstraint("CK_Preset_device_map_G", "[G] >= 0 AND [G] <= 255"));

            modelBuilder.Entity<Preset_device_map>()
                .ToTable(t => t.HasCheckConstraint("CK_Preset_device_map_B", "[B] >= 0 AND [B] <= 255"));

            modelBuilder.Entity<Preset_device_map>()
                .ToTable(t => t.HasCheckConstraint("CK_Preset_device_map_Brightness", "[Brightness] >= 0 AND [Brightness] <= 255"));

        }
    }
}
