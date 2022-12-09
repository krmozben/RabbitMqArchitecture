using Consumer.Model.Bus;
using MessageBroker.Message;

namespace Consumer.MessageHandlers
{
    public class ExampleHandler : IMessageHandler<ExampleModel>
    {
        public async Task HandleAsync(ExampleModel message, CancellationToken ct = default)
        {
            if (true)
            {
                Console.WriteLine(message.isSuccess);
            }
            else
            {
                throw new Exception("hata ver");
            }
        }
    }
}
