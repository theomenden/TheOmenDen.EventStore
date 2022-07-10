using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using TheOmenDen.EventStore.Identities;
using TheOmenDen.EventStore.Persistence;

namespace TheOmenDen.EventStore.Infrastructure;
internal sealed class EventStore: IEventStore
{
    private readonly string _offlineFolderPath;

    private readonly IIdentityService _identityService;

    private readonly IDbContextFactory<EventStoreContext> _dbContextFactory;

    public EventStore(IDbContextFactory<EventStoreContext> contextFactory, IIdentityService identityService)
    {
        var path = Directory.GetCurrentDirectory();

        var offlineInformation = Directory.CreateDirectory($"{path}/offline");

        _offlineFolderPath = offlineInformation.FullName;

        _identityService = identityService;

        _dbContextFactory = contextFactory;
    }

    public bool Exists(Guid aggregate)
    {
        using var context = _dbContextFactory.CreateDbContext();

        var doesExist = context.SerializedEvents
            .Any(e => e.Id == aggregate);

        return doesExist;
    }

    public async ValueTask<bool> ExistsAsync(Guid aggregate, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var doesExist = await context.SerializedEvents
            .AnyAsync(e => e.Id == aggregate, cancellationToken);

        return doesExist;
    }

    public bool Exists(Guid aggregate, int version)
    {
        using var context = _dbContextFactory.CreateDbContext();

        var doesExist = context.SerializedEvents
            .Any(e => e.Id == aggregate && e.MajorVersion == version);

        return doesExist;
    }

    public async ValueTask<bool> ExistsAsync(Guid aggregate, int version, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var doesExist = await context.SerializedEvents.AnyAsync(e => e.Id == aggregate && e.MajorVersion == version, cancellationToken);

        return doesExist;
    }

    public IEnumerable<IEvent> Get(Guid aggregate, int majorVersion)
    {
        using var context = _dbContextFactory.CreateDbContext();

        var events = context.SerializedEvents
            .Where(e => e.Id == aggregate)
            .Where(e => e.MajorVersion == majorVersion)
            .OrderByDescending(e => e.MinorVersion);
        
        return events.ToArray(); 
    }

    public async IAsyncEnumerable<IEvent> GetAsync(Guid aggregate, int majorVersion,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            yield break;
        }

        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        await foreach (var @event in context.SerializedEvents
                           .Where(e => e.Id == aggregate)
                           .Where(e => e.MajorVersion == majorVersion)
                           .OrderByDescending(e => e.MinorVersion)
                           .ToAsyncEnumerable()
                           .WithCancellation(cancellationToken))
        {
            yield return @event;
        }
    }

    public IEnumerable<Guid> GetExpired(DateTimeOffset expiredAt)
    {
        using var context = _dbContextFactory.CreateDbContext();

        var eventIds = context.SerializedAggregates
            .AsNoTracking()
            .Where(x => x.ExpiresAt != null && x.ExpiresAt <= expiredAt)
            .Select(x => x.Id);

        return eventIds.ToArray();
    }

    public async IAsyncEnumerable<Guid> GetExpiredAsync(DateTimeOffset expiredAt,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
           yield break;
        }

        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        await foreach (var eventId in context.SerializedAggregates
                           .AsNoTracking()
                           .Where(x => x.ExpiresAt != null && x.ExpiresAt <= expiredAt)
                           .Select(x => x.Id)
                           .ToAsyncEnumerable()
                           .WithCancellation(cancellationToken))
        {
            yield return eventId;
        }
    }

    public void Save(IAggregateRoot aggregate, IEnumerable<IEvent> events)
    {
        var currentSession = _identityService.GetCurrent();

        var tenant = currentSession.Tentant;
        var user = currentSession.User;

        var serializedEvents = new List<SerializedEvent>(20);

        using var context = _dbContextFactory.CreateDbContext();

        if (!context.SerializedAggregates.Any(sa => sa.Id == aggregate.Id))
        {
            var aggregateToAdd = new SerializedAggregate
            {
                TenantIdentifier = tenant.Id,
                Id = aggregate.Id,
                Type = aggregate.GetType().Name.Replace("Aggregate", string.Empty),
                Class = aggregate.GetType().GetClassName(),
            };

            context.SerializedAggregates.Add(aggregateToAdd);

            context.SaveChanges();
        }

        serializedEvents
            .AddRange(events
                .Select(@event => 
                    @event.Serialize(aggregate.Id, aggregate.MinorVersion, aggregate.MajorVersion, tenant.Id, user.Id))
            );

        context.SerializedEvents.AddRange(serializedEvents);

        context.SaveChanges();
    }

    public async Task SaveAsync(IAggregateRoot aggregate, IEnumerable<IEvent> events,
        CancellationToken cancellationToken = default)
    {
        var currentSession = await _identityService.GetCurrentAsync(cancellationToken);

        var tenant = currentSession.Tentant;
        var user = currentSession.User;

        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        if (!await context.SerializedAggregates.AnyAsync(sa => sa.Id == aggregate.Id,cancellationToken))
        {
            var aggregateToAdd = new SerializedAggregate
            {
                TenantIdentifier = tenant.Id,
                Id = aggregate.Id,
                Type = aggregate.GetType().Name.Replace("Aggregate", string.Empty),
                Class = aggregate.GetType().GetClassName(),
            };

            await context.SerializedAggregates.AddAsync(aggregateToAdd, cancellationToken);

            await context.SaveChangesAsync(cancellationToken);
        }

        var serializedEvents = new List<SerializedEvent>(20);

        serializedEvents
            .AddRange(events
                .Select(@event =>
                    @event.Serialize(aggregate.Id, aggregate.MinorVersion, aggregate.MajorVersion, tenant.Id, user.Id))
            );

        await context.SerializedEvents.AddRangeAsync(serializedEvents, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
    }

    public void Box(Guid aggregate)
    {
        throw new NotImplementedException();
    }

    public Task BoxAsync(Guid aggregate, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}

