using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace rumos_client.Models
{
    public class Device
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; } = String.Empty;
        [JsonPropertyName("ip_v4")]
        public string? Ip_v4 {  get; set; }
        [JsonPropertyName("platform_id")]
        public int Platform_id { get; set; }
        [JsonPropertyName("room_id")]
        public int Room_Id { get; set; }
    }

    public class LedColor
    {
        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }
        public int Brightness { get; set; } = 255;
    }

    public class DeviceState
    {
        public string Id { get; set; }
        public bool Success { get; set; }
    }
}
