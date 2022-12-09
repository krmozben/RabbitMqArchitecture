using MessageBroker.Message;
using RabbitMQ.Client;

namespace MessageBroker.Bus
{
    public interface IBusPublisher
    {
        Task PublishAsync<T>(T message, IBasicProperties? properties = null) where T : PublishedMessage;
    }
}
