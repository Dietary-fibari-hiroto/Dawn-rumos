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
}
