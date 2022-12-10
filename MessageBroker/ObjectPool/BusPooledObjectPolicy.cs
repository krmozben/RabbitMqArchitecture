using MessageBroker.Configuration;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace MessageBroker.ObjectPool
{
    /// <summary>
    /// https://www.c-sharpcorner.com/article/publishing-rabbitmq-message-in-asp-net-core/
    /// </summary>
    public class BusPooledObjectPolicy : IPooledObjectPolicy<IModel>
    {
        private readonly BusConfiguration _configuration;

        private readonly IConnection _connection;

        public BusPooledObjectPolicy(IOptions<BusConfiguration> configuration)
        {
            _configuration = configuration.Value;
            _connection = GetConnection();
        }

        private IConnection GetConnection()
        {
            var factory = new ConnectionFactory()
            {
                HostName = _configuration.HostName,
                UserName = _configuration.UserName,
                Password = _configuration.Password,
                Port = _configuration.Port,
                VirtualHost = _configuration.VHost,
                DispatchConsumersAsync = true
            };

            return factory.CreateConnection();
        }

        public IModel Create()
        {
            return _connection.CreateModel();
        }

        public bool Return(IModel obj)
        {
            if (obj.IsOpen)
            {
                return true;
            }
            else
            {
                obj?.Dispose();
                return false;
            }
        }
    }
}
