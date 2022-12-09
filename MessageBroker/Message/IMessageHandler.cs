namespace MessageBroker.Message;

public interface IMessageHandler<TMessage> where TMessage : IMessage
{
    Task HandleAsync(TMessage message, CancellationToken ct = default);
}
