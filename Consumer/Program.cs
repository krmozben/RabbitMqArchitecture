using Consumer;
using MessageBroker.Extension;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext,services) =>
    {
        services.AddBus(hostContext.Configuration, typeof(Program).Assembly);
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
