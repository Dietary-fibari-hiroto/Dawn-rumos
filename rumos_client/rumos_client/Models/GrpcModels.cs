using System.Text.Json.Serialization;

namespace rumos_client.Models
{
    public record RSetDeviceRequest(string ip);
    public record RSetPowerReply
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }
        [JsonPropertyName("message")]
        public string Message { get; set; }= string.Empty;
        [JsonPropertyName("isOn")]
        public bool IsOn { get; set; }
    }
    public record class RGetStatusReply
    {
        [JsonPropertyName("isConnect")]
        public bool IsConnect { get; set; }
        [JsonPropertyName("isOn")]
        public bool IsOn { get; set; }
        public RGetStatusReply(bool isConnect, bool isOn)
        {
            IsConnect = isConnect;
            IsOn = isOn;
        }
    }

    public record RSetAllDeviceRequest(string json);
    public record RSetAllDeviceReply(bool Success);
}
