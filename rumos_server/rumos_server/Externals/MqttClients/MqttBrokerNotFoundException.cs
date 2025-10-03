namespace rumos_server.Externals.MqttClients
{
    public class MqttBrokerNotFoundException:Exception
    {
        public MqttBrokerNotFoundException()
        {
        }

        public MqttBrokerNotFoundException(string message)
            : base(message)
        {
        }
        public MqttBrokerNotFoundException(string message, Exception innerException)
    : base(message, innerException)
        {
        }
    }
}
