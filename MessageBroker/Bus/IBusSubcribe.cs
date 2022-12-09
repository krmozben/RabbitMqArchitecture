using MessageBroker.Enums;
using MessageBroker.Message;

namespace MessageBroker.Bus;

public interface IBusSubcribe
{
    Task SubcribeAsync<TMessage>(string exchangeName, string queueName, string routingKey, ExchangeTypes exchangeType = ExchangeTypes.Unknown, ushort prefetchCount = 1, CancellationToken ct = default) where TMessage : IMessage;
}
