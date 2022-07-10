

namespace TheOmenDen.EventStore.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    private static readonly ILoggerFactory LoggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
    {
        builder.AddConsole();
    });

    public static IServiceCollection AddEventStoreServices<T>(this IServiceCollection services, Boolean shouldAddAdapters = true)
    {
        services.AddLazyCache();

        services.AddScoped<IEventQueue, EventQueue>();
        
        services.AddScoped<IEventRepository, EventRepository>();

        services.AddScoped<ICommandQueue, CommandQueue>();

        if (shouldAddAdapters)
        {
            AddMediatRAdapters<T>(services);
        }

        return services;
    }

    public static IServiceCollection AddEventStoreSqlServerServices(this IServiceCollection services, String databaseConnectionString)
    {
        services.AddPooledDbContextFactory<EventStoreContext>(options =>
            options.UseSqlServer(databaseConnectionString)
                .UseLoggerFactory(LoggerFactory)
#if DEBUG
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging()
#endif
                .EnableServiceProviderCaching()
        );

        return services;
    }

    public static IServiceCollection AddEventStoreSqlServerServices<TContext>(this IServiceCollection services, String databaseConnectionString)
    where TContext: DbContext
    {
        //EventStoreContext
        services.AddPooledDbContextFactory<TContext>(options =>
        options.UseSqlServer(databaseConnectionString)
            .UseLoggerFactory(LoggerFactory)
#if DEBUG
            .EnableDetailedErrors()
            .EnableSensitiveDataLogging()
#endif
            .EnableServiceProviderCaching()
        );

        return services;
    }

    private static IServiceCollection AddMediatRAdapters<T>(this IServiceCollection services)
    {
        services.AddMediatR(typeof(IMediator).Assembly, typeof(T).Assembly);

        services.AddTransient<MediatrToEventStoreAdapter>(provider =>
            {
                var mediator = provider.GetService<IMediator>();
                var commandQueue = provider.GetService<ICommandQueue>();
                return new (commandQueue!,mediator!);
            }
        );

        return services;
    }
}
