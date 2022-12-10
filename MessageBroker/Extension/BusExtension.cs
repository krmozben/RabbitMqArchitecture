using MessageBroker.Bus;
using MessageBroker.Configuration;
using MessageBroker.Message;
using MessageBroker.ObjectPool;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;
using RabbitMQ.Client;
using System.Reflection;

namespace MessageBroker.Extension;

public static class BusExtension
{
    public static IServiceCollection AddBus(this IServiceCollection services, IConfiguration configuration, params Assembly[] assemblies)
    {
        services.Configure<BusConfiguration>(configuration.GetSection("BusConfiguration"));
        services.AddSingleton<IBusSubcribe, BusSubcribe>();
        services.AddSingleton<IBusPublisher, BusPublisher>();
		services.AddHandlers(assemblies);
		services.AddSingleton<IPooledObjectPolicy<IModel>, BusPooledObjectPolicy>();

		return services;
    }

	private static IServiceCollection AddHandlers(this IServiceCollection services, params Assembly[] assemblies)
	{
		List<Type> types = new();

		foreach (var assembly in assemblies)
		{
			types.AddRange(Assembly.Load(assembly.GetName().Name ?? string.Empty).GetExportedTypes());
		}

		var messageHandlers = (from t in types
							   from i in t.GetInterfaces()
							   where i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMessageHandler<>)
							   select new
							   {
								   IMessageHandler = i,
								   MessageHandler = t
							   }).ToArray();

		foreach (var messageHandler in messageHandlers)
		{
			services.AddScoped(messageHandler.IMessageHandler, messageHandler.MessageHandler);
		}

		return services;
	}
}
