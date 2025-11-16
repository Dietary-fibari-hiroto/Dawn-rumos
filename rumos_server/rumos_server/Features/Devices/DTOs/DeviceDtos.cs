using rumos_server.Features.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace rumos_server.Features.DTOs
{
    //デバイス情報をPlatformと一緒に返す
    public class PlatformWithDevicesDto
    {
        public int PlatformId { get; set; }
        public string PlatformName { get; set; } = string.Empty;
        public List<DeviceDto> Devices { get; set; } = new();
    }

    public class DeviceDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Series { get; set; } 
        public bool IsPower { get; set; } = false;
        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }
        public int Brightness { get; set; }
    }
    public class CreateDeviceDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Ip_v4 { get; set; }
        public int Platform_id { get; set; }
        public int Room_id { get; set; }
    }
}
    
