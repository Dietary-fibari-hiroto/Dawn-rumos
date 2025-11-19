namespace rumos_server.Features.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


public class Platform
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int Id { get; set; }
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public ICollection<Device> Devices { get; set; } = new List<Device>();
}
public class Room
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class Device
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int Id { get; set; }
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;
    [StringLength(15)]
    public string? Ip_v4 { get; set; }

    [Required]
    [Column("platform_id")]
    public int Platform_id { get; set; }
    [ForeignKey(nameof(Platform_id))]
    public Platform? Platform { get; set; }


    [Column("room_id")]
    public int Room_id { get; set; }
    [ForeignKey(nameof(Room_id))]
    public Room? Room { get; set; }
}

//プリセットテーブルのデータ型
public class Preset {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int Id { get; set; }
    [StringLength(15)]
    public string Name { get; set; }=string.Empty;
    [StringLength(255)]
    public string Img_url { get; set; } = "";
}

public class Mode {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int Id { get; set; }

    [StringLength(255)]
    [Required]
    public string Name { get; set; } = string.Empty;
}


[Index(nameof(Preset_id),nameof(Device_id),IsUnique=true)]
public class Preset_device_map
{
    [Required]
    [Column("preset_id")]
    public int Preset_id { get; set; }
    [ForeignKey(nameof(Preset_id))]
    public Preset? Preset { get; set; }

    [Required]
    [Column("device_id")]
    public int Device_id { get; set; }
    [ForeignKey(nameof(Device_id))]
    public Device? Device { get; set; }

    public int R { get; set; } = 255;
    public int G { get; set; } = 255;
    public int B { get; set; } = 255;
    public int Brightness { get; set; } = 255;

    [Column("mode_id")]
    public int? Mode_id { get; set; } = 1;
    [ForeignKey(nameof(Mode_id))]
    public Mode? Mode { get; set; }
}
