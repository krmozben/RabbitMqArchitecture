using MessageBroker.Message;

namespace Consumer.Model.Bus
{
    public class ExampleModel : IMessage
    {
        public Guid MessageId { get; set; }
        public bool isSuccess { get; set; }
    }
}
