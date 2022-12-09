using MessageBroker.Message;

namespace Publisher.Model.Bus
{
    public class TestPublish : PublishedMessage
    {
        public TestPublish(string exchangeName, string routingKey) : base(exchangeName, routingKey)
        {
        }

        public int Number { get; set; }
        public string Text { get; set; }
    }
}
