using System;
using System.Text;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;

namespace MqTest
{
    class MQTT_Test
    {
        static async Task Req_ESP(string[] args)
        {
            //MQTTブローカーのアドレス
            string brokerAddress = "localhost";
            int brokerPort = 1883;
            string topic = "esp32/led";
            //クライアントオプション
            var options = new MqttClientOptionsBuilder()
                .WithTcpServer(brokerAddress, brokerPort)
                .WithClientId("AspNetPublisher")
                .Build();

            //MQTTクライアント生成
            var factory = new MqttFactory();
            var mqttClient = factory.CreateMqttClient();

            //接続
            await mqttClietnt.ConnectAsync(options);
            Console.WriteLine("Connected to MQTT broker.");

            int r = 255, g = 100, b = 50;
            string messagePayload = $"{r},{g},{b}";

            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(messagePayload)
                .WithAtMostOnceQoS()
                .Build();

            await mqttClient.PublishAsync(message);
            Console.WriteLine($"Published: {messagePayload}");

            await mqttClient.DisconnectAsync();
        }
    }
}