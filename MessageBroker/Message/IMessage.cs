namespace MessageBroker.Message
{
    public interface IMessage
    {
        public Guid MessageId { get; set; }
    }
}