namespace rumos_server.Features.Models;
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
    public int  Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Ip_v4 {  get; set; } = "";
    [Column("platform_id")]
    public int Platform_id { get; set; }
    [ForeignKey("platform_id")]
    public Platform? Platform { get; set; }

    [Column("room_id")]
    public int Room_id  { get; set; }
    [ForeignKey("platform_id")]
    public Room? Room { get; set; }
}