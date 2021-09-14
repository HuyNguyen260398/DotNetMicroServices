using System;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using PlatformService.Dtos;
using RabbitMQ.Client;

namespace PlatformService.AsyncDataServices
{
    public class MessageBusClient : IMessageBusClient
    {
        private readonly IConfiguration config;
        private readonly IConnection connection;
        private readonly IModel channel;

        public MessageBusClient(IConfiguration config)
        {
            this.config = config;
            var factory = new ConnectionFactory() { HostName = this.config["RabbitMQHost"], 
                Port = int.Parse(this.config["RabbitMQPort"]) };

            try
            {
                connection = factory.CreateConnection();
                channel = connection.CreateModel();
                channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
                connection.ConnectionShutdown += RabbitMQConnectionShutdown;

                Console.WriteLine("Connected to Message Bus");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not connect to Message Bus: {e.Message}");
            }
        }

        public void PublishNewPlatform(PlatformPublishedDto platformPublishedDto)
        {
            var message = JsonSerializer.Serialize(platformPublishedDto);

            if (connection.IsOpen)
            {
                Console.WriteLine("RabbitMQ connection is opened, sending message");

                // To do send the message
                SendMessage(message);
            }
            else
            {
                Console.WriteLine("RabbitMQ connection is closed, not sending message");
            }
        }

        private void RabbitMQConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine("RabbitMQ Connection Shutdown");
        }

        private void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "trigger",
                routingKey: "",
                basicProperties: null,
                body: body);

            Console.WriteLine($"Message sent: {message}");
        }

        public void Dispose()
        {
            Console.WriteLine("Message Bus disposed");

            if (channel.IsOpen)
            {
                channel.Close();
                connection.Close();
            }
        }
    }
}