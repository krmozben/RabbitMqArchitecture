using MessageBroker.Message;

namespace Consumer.Model.Bus
{
    public class TestModel : IMessage
    {
        public Guid MessageId { get; set; }
        public int Number { get; set; }
        public string Text { get; set; }
    }
}
