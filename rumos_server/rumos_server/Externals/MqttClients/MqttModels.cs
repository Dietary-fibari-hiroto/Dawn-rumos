namespace rumos_server.Externals.MqttClients
{
    public sealed class LedColor
    {
        public int R {  get; set; }
        public int G { get; set; }
        public int B { get; set; }
        public int Brightness   { get; set; }
        public string Mode { get; set; } = "normal";
    }
}
