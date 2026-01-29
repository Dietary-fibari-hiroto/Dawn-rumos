
namespace rumos_server.Externals.IoTHubClients
{
    public sealed class IoTHubSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public int MessageTimeoutSeconds { get; set; } = 60;
    }
}
