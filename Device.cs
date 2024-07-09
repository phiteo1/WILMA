using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using Newtonsoft.Json.Linq;

namespace RPCClient
{
    internal class Device
    {

        readonly private string deviceID;
        readonly private string username;
        readonly private string password;
        private IMqttClient mqttClient;
        public Device(string DeviceID, string Username, string Password = "") { 
            
            deviceID = DeviceID;
            username = Username;
            password = Password;
            var mqttFactory = new MqttFactory();
            mqttClient = mqttFactory.CreateMqttClient();

        }

        private async Task createMQTTConnection()
        {
            
                // Use builder classes where possible in this project.
            var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithTcpServer("192.168.1.60", 1883)
                .WithClientId(DeviceID)
                .WithCredentials(username,password)
                .Build();

            // This will throw an exception if the server is not available.
            // The result from this message returns additional data which was sent 
            // from the server. Please refer to the MQTT protocol specification for details.
            var response = await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

            Console.WriteLine("The MQTT client is connected.");

            response.DumpToConsole();

        }

        private async Task subscribeToRPC()
        {
            await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("v1/devices/me/rpc/request/+").Build());
            Console.WriteLine("Subscribed to RPC requests topic.");

        }

        public async Task daemonDevice()
        {
            await createMQTTConnection();
            await subscribeToRPC();

            mqttClient.ApplicationMessageReceivedAsync += e =>
            {
                var topic = e.ApplicationMessage.Topic;
                Console.WriteLine($"Received message on topic {topic}");
                var payload = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
                Console.WriteLine($"Received message on topic {topic}: {payload}");

                var request = JObject.Parse(payload);
                var requestId = topic.Split('/')[4];  // Extract the request id
                var method = request["method"].ToString();
                var response = new JObject { ["canStart"] = 0 }.ToString();
                if (method == "StartMeasure")
                {
                    var responseTopic = $"v1/devices/me/telemetry";
                    var message = new MqttApplicationMessageBuilder()
                        .WithTopic(responseTopic)
                        .WithPayload(response)
                        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                        .Build();
                    mqttClient.PublishAsync(message);
                    System.Threading.Thread.Sleep(4000);
                    Console.WriteLine("Test");
                    response = new JObject { ["canStart"] = 1 }.ToString();
                    responseTopic = $"v1/devices/me/telemetry";
                    message = new MqttApplicationMessageBuilder()
                            .WithTopic(responseTopic)
                            .WithPayload(response)
                            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                            .Build();
                    mqttClient.PublishAsync(message);
                }

                response.DumpToConsole();
                return Task.CompletedTask;
            };


            
            Console.ReadLine();

        }
        public string Password => password;

        public string Username => username;

        public string DeviceID => deviceID;
    }
}
