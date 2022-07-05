using Microsoft.Extensions.DependencyInjection;

namespace TheOmenDen.EventStore.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEventStoreServices(this IServiceCollection services)
    {
        services.AddSingleton<IEventStream, EventStreamBase>();

        return services;
    }
}
