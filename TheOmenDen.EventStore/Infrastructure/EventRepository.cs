namespace TheOmenDen.EventStore.Infrastructure;
public class EventRepository : IEventRepository
{
    private readonly IEventStore _store;

    public EventRepository(IEventStore store) => _store = store;
    #region Getters
    public T Get<T>(Guid id) where T : IAggregateRoot
    {
        return Rehydrate<T>(id);
    }

    public async ValueTask<T> GetAsync<T>(Guid id, CancellationToken cancellationToken = new CancellationToken()) where T : IAggregateRoot
    {
        return await RehydrateAsync<T>(id, cancellationToken);
    }
    #endregion
    #region Save Methods
    public IEvent[] Save<T>(T aggregate, int? version = null) where T : IAggregateRoot
    {
        if (version != null && (_store.Exists(aggregate.Id, version.Value)))
        {
            throw new ConcurrencyException(aggregate.Id);
        }

        var events = aggregate.FlushUncommittedChanges();

        _store.Save(aggregate, events);

        return events;
    }

    public async ValueTask<IEvent[]> SaveAsync<T>(T aggregate, int? version = null, CancellationToken cancellationToken = new CancellationToken()) where T : IAggregateRoot
    {
        if (version != null && await _store.ExistsAsync(aggregate.Id, version.Value, cancellationToken))
        {
            throw new ConcurrencyException(aggregate.Id);
        }

        var events = await aggregate.FlushUncommittedChangesAsync(cancellationToken).ToArrayAsync(cancellationToken);

        await _store.SaveAsync(aggregate, events, cancellationToken);

        return events;
    }
    #endregion
    #region Boxing // Unboxing
    public void Box<T>(T aggregate) where T : IAggregateRoot
    {
        throw new NotImplementedException();
    }

    public Task BoxAsync<T>(T aggregate, CancellationToken cancellationToken = new CancellationToken()) where T : IAggregateRoot
    {
        throw new NotImplementedException();
    }

    public T Unbox<T>(Guid aggregate) where T : IAggregateRoot
    {
        throw new NotImplementedException();
    }

    public ValueTask<T> UnboxAsync<T>(Guid aggregate) where T : IAggregateRoot
    {
        throw new NotImplementedException();
    }
    #endregion
    #region Private Methods
    private T Rehydrate<T>(Guid id) where T : IAggregateRoot
    {
        var events = _store.Get(id, -1).ToList();

        if (!events.Any())
        {
            throw new AggregateNotFoundException(typeof(T), id);
        }

        var aggregate = AggregateFactory<T>.CreateAggregate();

        aggregate.Rehydrate(events);

        return aggregate;
    }

    private async ValueTask<T> RehydrateAsync<T>(Guid id, CancellationToken cancellationToken = default)
        where T : IAggregateRoot
    {
        var events = await _store
            .GetAsync(id, -1, cancellationToken)
            .ToListAsync(cancellationToken);

        if (!events.Any())
        {
            throw new AggregateNotFoundException(typeof(T), id);
        }

        var aggregate = AggregateFactory<T>.CreateAggregate();

        await aggregate.RehydrateAsync(events, cancellationToken);

        return aggregate;
    }
    #endregion
}
