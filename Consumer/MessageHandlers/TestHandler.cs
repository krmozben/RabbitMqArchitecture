using Consumer.Model.Bus;
using MessageBroker.Message;

namespace Consumer.MessageHandlers
{
    public class TestHandler : IMessageHandler<TestModel>
    {
        public async Task HandleAsync(TestModel message, CancellationToken ct = default)
        {
            if (new Random().Next(100) < 50)
            {
                Console.WriteLine(message.Text + " " + message.Number);
            }
            else
            {
                throw new Exception("hata ver");
            }
        }
    }
}
