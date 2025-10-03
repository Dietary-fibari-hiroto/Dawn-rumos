namespace rumos_server.Features.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Platform
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
public class Room
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class Device
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Ip_v4 { get; set; }

    [Column("platform_id")]
    public int Platform_id { get; set; }

    [ForeignKey("Platform_id")]
    public Platform? Platform { get; set; }

    [Column("room_id")]
    public int Room_id { get; set; }

    [ForeignKey("Room_id")]
    public Room? Room { get; set; }
}

//プリセットテーブルのデータ型
public class Preset {
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }=string.Empty;
    public string Img_url { get; set; } = "";
}

public class Preset_device_map
{
    [Column("preset_id")]
    public int Preset_id { get; set; }
    [ForeignKey("Preset_id")]
    public Preset? Preset { get; set; }

    [Column("device_id")]
    public int Device_id { get; set; }
    [ForeignKey("Device_id")]
    public Device? Device { get; set; }

    public int R { get; set; } = 255;
    public int G { get; set; } = 255;
    public int B { get; set; } = 255;
    public int Brightness { get; set; } = 255;

    public string mode { get; set; } = "normal";
}
