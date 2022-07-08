using TheOmenDen.EventStore.Infrastructure.Snapshots;

namespace TheOmenDen.EventStore.Interfaces;

public interface IStore
{
    void Save(Snapshot snapshot);

    ValueTask<bool> SaveAsync(Snapshot snapshot, CancellationToken cancellationToken = default);


    void Box(Guid id);

    ValueTask<bool> BoxAsync(Guid id, CancellationToken cancellationToken = default);

    Snapshot Unbox(Guid id, Type aggregateRootType);

    ValueTask<Snapshot> UnboxAsync(Guid id, Type aggregateRootType, CancellationToken cancellationToken = default);
}

