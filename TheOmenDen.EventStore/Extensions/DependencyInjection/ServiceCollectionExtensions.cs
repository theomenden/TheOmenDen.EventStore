using Microsoft.Extensions.DependencyInjection;

namespace TheOmenDen.EventStore.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEventStoreServices<T>(this IServiceCollection services)
    {
        var containerType = typeof(T);

        services.AddSingleton<IEventStream, EventStreamBase>();

        services.Scan(scan => scan.FromAssembliesOf(typeof(IEventSubscriber<,>), containerType)
            .AddClasses(c => c.AssignableTo(typeof(EventHandler<,>)))
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        services.Scan(scan => scan.FromAssembliesOf(typeof(IAsyncEventSubscriber<,>), containerType)
            .AddClasses(c => c.AssignableTo(typeof(AsyncEventHandler<,>)))
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        services.Scan(scan => scan.FromAssembliesOf(typeof(IEventPublisher<,>), containerType)
            .AddClasses(c => c.AssignableTo(typeof(EventPublisher<,>)))
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        services.Scan(scan => scan.FromAssembliesOf(typeof(IAsyncEventPublisher<,>), containerType)
            .AddClasses(c => c.AssignableTo(typeof(AsyncEventPublisher<,>)))
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        return services;
    }
}
