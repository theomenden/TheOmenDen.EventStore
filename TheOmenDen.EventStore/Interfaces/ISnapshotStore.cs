using TheOmenDen.EventStore.Infrastructure.Snapshots;
namespace TheOmenDen.EventStore.Interfaces;

public interface ISnapshotStore
{
    void Save(Snapshot snapshot);

    Task SaveAsync(Snapshot snapshot, CancellationToken cancellationToken = new());

    Snapshot Get(Guid id, Type underlyingType);

    ValueTask<Snapshot> GetAsync(Guid id, Type underlyingType, CancellationToken cancellationToken = new());

    #region Boxing|Unboxing
    void Box(Guid aggregateId);

    Task BoxAsync(Guid aggregateId, CancellationToken cancellationToken = new());

    Snapshot Unbox(Guid aggregateId, Type underlyingType);

    ValueTask<Snapshot> UnboxAsync(Guid aggregateId, Type underlyingType, CancellationToken cancellationToken = new());
    #endregion
}