using Microsoft.Extensions.ObjectPool;
using ObjectPoolPatternExample;
using ObjectPoolPatternExample.Model;
using ObjectPoolPatternExample.Policy;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<IPooledObjectPolicy<Example>, ExamplPoolPolicy>();
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
