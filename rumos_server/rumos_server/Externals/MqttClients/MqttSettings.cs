namespace rumos_server.Externals.MqttClients
{
    public sealed class MqttSettings
    {
        public string Broker { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 1883;
        public string ClientId { get; set; } = $"api-{Environment.MachineName}-{Guid.NewGuid():N}";
        public string? Username { get; set; }
        public string? Password { get; set; }
        public bool UseTls { get; set; } = false;
        public bool CleanSession { get; set; } = false;
        public int KeepAliveSeconds { get; set; } = 60;
    }
}
