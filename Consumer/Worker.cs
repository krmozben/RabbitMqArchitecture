using Consumer.Constants;
using Consumer.Model.Bus;
using MessageBroker.Bus;

namespace Consumer
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
            var busSubcribe = scope.ServiceProvider.GetRequiredService<IBusSubcribe>();

            await busSubcribe.SubcribeAsync<TestModel>(BusConstants.ConsumerExchange, BusConstants.TestQueue, "", MessageBroker.Enums.ExchangeTypes.Direct, ct: stoppingToken);
            //await busSubcribe.SubcribeAsync<ExampleModel>(BusConstants.ConsumerExchange, BusConstants.ExampleQueue, "", MessageBroker.Enums.ExchangeTypes.Fanout, ct: stoppingToken);
        }
    }
}