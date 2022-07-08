namespace TheOmenDen.EventStore.Interfaces;
public interface IEventRepository
{
    T Get<T>(Guid id) where T : IAggregateRoot;

    ValueTask<T> GetAsync<T>(Guid id, CancellationToken cancellationToken = new()) where T : IAggregateRoot;

    IEvent[] Save<T>(T aggregate, int? version = null) where T : IAggregateRoot;

    ValueTask<IEvent[]> SaveAsync<T>(T aggregate, int? version = null, CancellationToken cancellationToken = new())
        where T : IAggregateRoot;

    void Box<T>(T aggregate) where T : IAggregateRoot;

    Task BoxAsync<T>(T aggregate, CancellationToken cancellationToken = new()) where T : IAggregateRoot;

    T Unbox<T>(Guid aggregate) where T : IAggregateRoot;

    ValueTask<T> UnboxAsync<T>(Guid aggregate) where T : IAggregateRoot;
}

