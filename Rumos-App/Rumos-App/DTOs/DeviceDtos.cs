
namespace Rumos_App.DTOs
{
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
        public string Series { get; set; } = string.Empty;
        public bool IsPower { get; set; } = false;
        public int R { get; set; } = 0;
        public int G { get; set; } = 0;
        public int B { get; set; } = 0;
        public int Brightness { get; set; } = 0;
    }
    public class DevicePostDto {
        public int R { get; set; } = 0;
        public int G { get; set; } = 0;
        public int B { get; set; } = 0;
        public int Brightness { get; set; } = 0;
        public string Mode { get; set; } = "normal";
    }
    public class CreateDeviceDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Ip_v4 { get; set; }
        public int Platform_id { get; set; }
        public int Room_id { get; set; }
    }

}
