using MessageBroker.Message;
using Microsoft.Extensions.ObjectPool;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace MessageBroker.Bus
{
    public class BusPublisher : IBusPublisher
    {
        private readonly DefaultObjectPool<IModel> _objectPool;

        public BusPublisher(IPooledObjectPolicy<IModel> objectPolicy)
        {
            _objectPool = new DefaultObjectPool<IModel>(objectPolicy);
        }

        public Task PublishAsync<T>(T message, IBasicProperties? properties = null) where T : PublishedMessage
        {
            if (message == null)
                return Task.CompletedTask;

            var _channel = _objectPool.Get();

            try
            {
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
            finally
            {
                _objectPool.Return(_channel);
            }

            return Task.CompletedTask;
        }
    }
}
