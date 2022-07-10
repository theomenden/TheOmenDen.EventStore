using TheOmenDen.EventStore.Caching;

namespace TheOmenDen.EventStore.Infrastructure.Snapshots;
internal sealed class SnapshotRepository : IEventRepository
{
    #region Private Fields
    private readonly IAppCache _appCache;
    private readonly GuidCache<IAggregateRoot> _cache = new ();
    private readonly ISnapshotStore _snapshotStore;
    private readonly ISnapshotStrategy _snapshotStrategy;
    private readonly IEventRepository _eventRepository;
    private readonly IEventStore _eventStore;
    #endregion
    public SnapshotRepository(IAppCache appCache,IEventStore eventStore, IEventRepository eventRepository, ISnapshotStore snapshotStore, ISnapshotStrategy snapshotStrategy)
    {
        _appCache = appCache;
        _eventStore = eventStore;
        _eventRepository = eventRepository;
        _snapshotStore = snapshotStore;
        _snapshotStrategy = snapshotStrategy;
    }
    #region Saving Methods
    public IEvent[] Save<T>(T aggregate, int? version = null) 
        where T : IAggregateRoot
    {
        // Cache the aggregate for 5 minutes.
        _cache.Add(aggregate.Id, aggregate, 300, true);

        // Take a snapshot if needed.
        TakeSnapshot(aggregate, false);

        // Return the stream of saved events to the caller so they can be published.
        return _eventRepository.Save(aggregate, version);
    }

    public ValueTask<IEvent[]> SaveAsync<T>(T aggregate, int? version = null, CancellationToken cancellationToken = new CancellationToken()) where T : IAggregateRoot
    {
        throw new NotImplementedException();
    }
    #endregion
    #region Get Methods
    public T Get<T>(Guid aggregateId) 
        where T : IAggregateRoot
    {
        // If it is not in the cache then load the aggregate from the most recent snapshot.
        var aggregate = AggregateFactory<T>.CreateAggregate();
        var snapshotVersion = RestoreAggregateFromSnapshot(aggregateId, aggregate);

        // If there is no snapshot then load the aggregate directly from the event store.
        if (snapshotVersion == -1)
        {
            return _eventRepository.Get<T>(aggregateId);
        }

        // Otherwise load the aggregate from the events that occurred after the snapshot was taken.
        var events = _eventStore
            .Get(aggregateId, snapshotVersion)
            .Where(desc => desc.MajorVersion > snapshotVersion)
            .OrderByDescending(desc => desc.MajorVersion)
            .ThenByDescending(desc => desc.MinorVersion);

        aggregate.Rehydrate(events);

        return aggregate;
    }

    public async ValueTask<T> GetAsync<T>(Guid id, CancellationToken cancellationToken = default)
        where T : IAggregateRoot
    {
        var snapshot = _cache.Get(id);

        if (snapshot is not null)
        {
            return (T)snapshot;
        }

        var aggregate = AggregateFactory<T>.CreateAggregate();
        
        var snapshotVersion = await RestoreAggregateFromSnapshotAsync(id, aggregate, cancellationToken);
        
        var events = await _eventStore
            .GetAsync(id, snapshotVersion, cancellationToken)
            .Where(desc => desc.MajorVersion > snapshotVersion)
            .OrderByDescending(desc => desc.MajorVersion)
            .ThenByDescending(desc => desc.MinorVersion)
            .ToArrayAsync(cancellationToken);
        
        await aggregate.RehydrateAsync(events, cancellationToken);

        return aggregate;
    }
    #endregion
    #region Private Aggregate Restoration
    private int RestoreAggregateFromSnapshot<T>(Guid id, T aggregate) 
        where T : IAggregateRoot
    {
        var snapshot = _snapshotStore.Get(id, typeof(T));

        if (snapshot is null)
        {
            return -1;
        }

        // TODO: Is this needed when only the version is returned?

         aggregate.Id = snapshot.AggregateIdentifier;
         aggregate.MajorVersion = snapshot.AggregateMajorVersion;
         aggregate.MinorVersion = snapshot.AggregateMinorVersion;
         aggregate.State = snapshot.AggregateState;

        return snapshot.AggregateMajorVersion;
    }

    private async ValueTask<int> RestoreAggregateFromSnapshotAsync<T>(Guid id, T aggregate, CancellationToken cancellationToken = default)
        where T : IAggregateRoot
    {
        var snapshot = await _snapshotStore.GetAsync(id, typeof(T), cancellationToken);

        if (snapshot is null)
        {
            return -1;
        }

        aggregate.Id = snapshot.AggregateIdentifier;
        aggregate.MajorVersion = snapshot.AggregateMajorVersion;
        aggregate.MinorVersion = snapshot.AggregateMinorVersion;
        aggregate.State = snapshot.AggregateState;

        return snapshot.AggregateMajorVersion;
    }
    #endregion
    #region Private Snapshot Taking Methods
    private void TakeSnapshot(IAggregateRoot aggregate, bool force)
    {
        if (!force && !_snapshotStrategy.ShouldTakeSnapshot(aggregate))
        {
            return;
        }

        var snapshot = new Snapshot
        {
            AggregateIdentifier = aggregate.Id,
            AggregateMajorVersion = aggregate.MajorVersion,
            AggregateState = aggregate.State,
            AggregateMinorVersion = aggregate.MinorVersion + aggregate.GetUncommittedChanges().Length
        };

        _snapshotStore.Save(snapshot);
    }

    private async Task TakeSnapshotAsync(IAggregateRoot aggregate, bool force, CancellationToken cancellationToken = default)
    {
        if (!force && !await _snapshotStrategy.ShouldTakeSnapshotAsync(aggregate, cancellationToken))
        {
            return;
        }

        var previousEventsInAggregate =
            await aggregate.GetUncommittedChangesAsync(cancellationToken).ToArrayAsync(cancellationToken);

        var snapshot = new Snapshot
        {
            AggregateIdentifier = aggregate.Id,
            AggregateMajorVersion = aggregate.MajorVersion,
            AggregateState = aggregate.State,
            AggregateMinorVersion = aggregate.MinorVersion + previousEventsInAggregate.Length
        };

        await _snapshotStore.SaveAsync(snapshot, cancellationToken);
    }
    #endregion
    #region Ping Methods
    public void Ping()
    {
        var aggregates = _eventStore.GetExpired(DateTimeOffset.UtcNow);
        foreach (var aggregate in aggregates)
        {
            Box(Get<IAggregateRoot>(aggregate));
        }
    }

    public async Task PingAsync(CancellationToken cancellationToken = default)
    {
        var aggregates = _eventStore.GetExpiredAsync(DateTimeOffset.UtcNow, cancellationToken);

        await foreach (var aggregate in aggregates.WithCancellation(cancellationToken))
        {
            var previousAggregate = await GetAsync<IAggregateRoot>(aggregate, cancellationToken);
            await BoxAsync(previousAggregate, cancellationToken);
        }
    }
    #endregion
    #region Boxing and Unboxing Methods
    public void Box<T>(T aggregate) where T : IAggregateRoot
    {
        TakeSnapshot(aggregate, true);

        _snapshotStore.Box(aggregate.Id);
        
        _eventStore.Box(aggregate.Id);

        _cache.Remove(aggregate.Id);
    }

    public async Task BoxAsync<T>(T aggregate, CancellationToken cancellationToken = new CancellationToken()) where T : IAggregateRoot
    {
        await TakeSnapshotAsync(aggregate, true, cancellationToken);

        await _snapshotStore.BoxAsync(aggregate.Id, cancellationToken);

        await _eventStore.BoxAsync(aggregate.Id, cancellationToken);

        _appCache.Remove(aggregate.Id.ToString());
    }

    public T Unbox<T>(Guid aggregateId) where T : IAggregateRoot
    {
        var snapshot = _snapshotStore.Unbox(aggregateId, typeof(T));
        
        var aggregate = AggregateFactory<T>.CreateAggregate();
        
        aggregate.Id = aggregateId;
        aggregate.MajorVersion = snapshot.AggregateMajorVersion;
        aggregate.MinorVersion = 1;
        aggregate.State = snapshot.AggregateState;
        
        return aggregate;
    }

    public async ValueTask<T> UnboxAsync<T>(Guid aggregateId)
        where T : IAggregateRoot
    {
        var snapshot = await _snapshotStore.UnboxAsync(aggregateId, typeof(T));

        var aggregate = AggregateFactory<T>.CreateAggregate();

        aggregate.Id = aggregateId;
        aggregate.MajorVersion = snapshot.AggregateMajorVersion;
        aggregate.MinorVersion = 1;
        aggregate.State = snapshot.AggregateState;

        return aggregate;
    }
    #endregion
}
