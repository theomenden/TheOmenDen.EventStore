using TheOmenDen.EventStore.Infrastructure.Snapshots;

namespace TheOmenDen.EventStore.Interfaces;

public interface IStore
{
    void Save(Snapshot snapshot);

    ValueTask<bool> SaveAsync(Snapshot snapshot, CancellationToken cancellationToken = new());

    void Box(Guid id);

    ValueTask<bool> BoxAsync(Guid id, CancellationToken cancellationToken = new());

    Snapshot Unbox(Guid id, Type aggregateRootType);

    ValueTask<Snapshot> UnboxAsync(Guid id, Type aggregateRootType, CancellationToken cancellationToken = new());
}

