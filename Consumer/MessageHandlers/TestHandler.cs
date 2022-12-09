using Consumer.Model.Bus;
using MessageBroker.Message;

namespace Consumer.MessageHandlers
{
    public class TestHandler : IMessageHandler<TestModel>
    {
        public async Task HandleAsync(TestModel message, CancellationToken ct = default)
        {
            if (true)
            {
                Console.WriteLine(message.Text);
            }
            else
            {
                throw new Exception("hata ver");
            }
        }
    }
}
