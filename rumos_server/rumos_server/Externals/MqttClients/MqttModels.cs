namespace rumos_server.Externals.MqttClients
{
    public sealed class LedColor
    {
        public int R { get; set; } = 0;
        public int G { get; set; } = 0;
        public int B { get; set; } = 0;
        public int Brightness   { get; set; } = 0;
        public string Mode { get; set; } = "normal";
    }
}
