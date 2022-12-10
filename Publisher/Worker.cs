using MessageBroker.Bus;
using Publisher.Constants;
using Publisher.Model.Bus;

namespace Publisher
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceProvider _serviceProvider;

        public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var busPublisher = scope.ServiceProvider.GetRequiredService<IBusPublisher>();

            var publishModel = new TestPublish(BusConstants.ConsumerFanoutExchange, "")
            {
                MessageId = Guid.NewGuid(),
                Number = 61,
                Text = "abc"
            };

            var publishModel2 = new ExamplePublish(BusConstants.ConsumerDirectExchange, BusConstants.ExampleRouteKey)
            {
                MessageId = Guid.NewGuid(),
                isSuccess = true,
            };

            await busPublisher.PublishAsync(publishModel);
            await busPublisher.PublishAsync(publishModel2);


            for (int i = 0; i <  500; i++)
            {

                var pm = new TestPublish(BusConstants.ConsumerFanoutExchange, "")
                {
                    MessageId = Guid.NewGuid(),
                    Number = i,
                    Text = "abc"
                };

                await busPublisher.PublishAsync(pm);
            }
        }
    }
}