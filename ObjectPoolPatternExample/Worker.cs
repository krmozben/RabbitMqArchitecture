using Microsoft.Extensions.ObjectPool;
using ObjectPoolPatternExample.Model;

namespace ObjectPoolPatternExample
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
            var policy = scope.ServiceProvider.GetRequiredService<IPooledObjectPolicy<Example>>();

            var objectPool = new DefaultObjectPool<Example>(policy);

            var obj1 = objectPool.Get();
            var obj2 = objectPool.Get();
            var obj3 = objectPool.Get();

            objectPool.Return(obj1);
            objectPool.Return(obj2);
            objectPool.Return(obj3);

            var obj4 = objectPool.Get();
            var obj5 = objectPool.Get();
            var obj6 = objectPool.Get();

            var obj7 = objectPool.Get();
            var obj8 = objectPool.Get();
            objectPool.Return(obj7);
            var obj9 = objectPool.Get();

        }
    }
}