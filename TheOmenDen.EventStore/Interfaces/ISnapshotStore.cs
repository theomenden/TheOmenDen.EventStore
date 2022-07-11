using TheOmenDen.EventStore.Infrastructure.Snapshots;
namespace TheOmenDen.EventStore.Interfaces;

/// <summary>
/// Defines retrieval and creation methods for <see cref="Snapshot"/>s, and allows for the moving of snapshots from a store to a boxed file
/// </summary>
public interface ISnapshotStore
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="snapshot"></param>
    void Save(Snapshot snapshot);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="snapshot"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task SaveAsync(Snapshot snapshot, CancellationToken cancellationToken = new());
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="underlyingType"></param>
    /// <returns></returns>
    Snapshot Get(Guid id, Type underlyingType);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="underlyingType"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    ValueTask<Snapshot> GetAsync(Guid id, Type underlyingType, CancellationToken cancellationToken = new());

    #region Boxing|Unboxing
    /// <summary>
    /// 
    /// </summary>
    /// <param name="aggregateId"></param>
    void Box(Guid aggregateId);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="aggregateId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task BoxAsync(Guid aggregateId, CancellationToken cancellationToken = new());
    /// <summary>
    /// 
    /// </summary>
    /// <param name="aggregateId"></param>
    /// <param name="underlyingType"></param>
    /// <returns></returns>
    Snapshot Unbox(Guid aggregateId, Type underlyingType);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="aggregateId"></param>
    /// <param name="underlyingType"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    ValueTask<Snapshot> UnboxAsync(Guid aggregateId, Type underlyingType, CancellationToken cancellationToken = new());
    #endregion
}