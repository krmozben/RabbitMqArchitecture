using System.Text.Json.Serialization;

namespace MessageBroker.Message
{
    public class PublishedMessage
    {
        private readonly string _exchangeName;
        private readonly string _routingKey;

        protected PublishedMessage(string exchangeName, string routingKey)
        {
            _exchangeName = exchangeName.ToLower();
            _routingKey = routingKey.ToLower();
        }

        [JsonIgnore]
        public string ExchangeName { get => _exchangeName; }

        [JsonIgnore]
        public string RoutingKey { get => _routingKey; }

        private Guid? _messageId;

        public Guid MessageId
        {
            get
            {
                return (_messageId = _messageId ?? Guid.NewGuid()).Value;
            }
            set { _messageId = value; }
        }
    }
}
