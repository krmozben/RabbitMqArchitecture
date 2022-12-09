using MessageBroker.Configuration;
using MessageBroker.Message;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace MessageBroker.Bus
{
    public class BusPublisher : IBusPublisher
    {
        private readonly BusConfiguration _configuration;
        private readonly IConnection _connection;

        public BusPublisher(IOptions<BusConfiguration> configuration)
        {
            _configuration = configuration.Value;

            var factory = new ConnectionFactory()
            {
                HostName = _configuration.HostName,
                VirtualHost = _configuration.VHost,
                UserName = _configuration.UserName,
                Password = _configuration.Password,
                Port = _configuration.Port
            };

            _connection = factory.CreateConnection();
        }

        public Task PublishAsync<T>(T message, IBasicProperties? properties = null) where T : PublishedMessage
        {
            if (message == null)
            {
                return Task.CompletedTask;
            }

            try
            {
                var _channel = _connection.CreateModel();

                if (properties == null)
                {
                    properties = _channel.CreateBasicProperties();
                    properties.Persistent = true;
                }

                properties.MessageId = message.ToString();

                var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

                _channel.BasicPublish(message.ExchangeName, message.RoutingKey, properties, bytes);
            }
            catch (Exception)
            {
                throw;
            }

            return Task.CompletedTask;
        }
    }
}
