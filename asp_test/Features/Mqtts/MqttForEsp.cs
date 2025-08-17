using MQTTnet;
using MQTTnet.Client;
using System.Text.Json;
using DotNetEnv;

namespace MqTest
{
public class MQTT_Test
{
    private readonly IMqttClient _mqttClient;

    public MQTT_Test()
    {
        var factory = new MqttFactory();
        _mqttClient = factory.CreateMqttClient();

        string BrokerIp = Environment.GetEnvironmentVariable("MQTTBROKER_IP");
        string portStr = Environment.GetEnvironmentVariable("MQTTBROKER_PORT");
        int BrokerPort = int.Parse(portStr);
            var options = new MqttClientOptionsBuilder()
            .WithTcpServer(BrokerIp,BrokerPort) // ブローカ PC の LAN IP
            .Build();

        _mqttClient.ConnectAsync(options, CancellationToken.None).Wait();
    }

    public async Task SendLedColorAsync(LedColor color)
    {
        var payload = JsonSerializer.Serialize(color);
        var message = new MqttApplicationMessageBuilder()
            .WithTopic("esp32/led")
            .WithPayload(payload)
            .Build();

        await _mqttClient.PublishAsync(message);
    }
}    public class LedColor
{
    public int R { get; set; }
    public int G { get; set; }
    public int B { get; set; }
}
}