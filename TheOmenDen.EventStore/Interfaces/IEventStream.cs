namespace TheOmenDen.EventStore.Interfaces;
/// <summary>
/// A simple interface that defines methods related to basic event sourcing patterns
/// </summary>
public interface IEventStream : IEnumerable<BaseEvent>,
    IAsyncEnumerable<KeyValuePair<Guid, List<BaseEvent>>>,
    IAsyncStreamAccessor<BaseEvent>,
    IAccessor<BaseEvent>
{
    /// <summary>
    /// Retrieves the total count of <see cref="BaseEvent"/>s in the store
    /// </summary>
    Int32 Count { get; }

    /// <summary>
    /// Retrieves the count of <see cref="BaseEvent"/>s for a particular aggregate in the store
    /// </summary>
    /// <param name="aggregateId">The aggregate to count events under</param>
    /// <returns>The count</returns>
    Int32 CountForAggregate(Guid aggregateId);

    /// <summary>
    /// Retrieves the total count of <see cref="BaseEvent"/>s in the store, asynchronously with cancellation support via <paramref name="cancellationToken"/>
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns><see cref="ValueTask{TResult}"/> of the count</returns>
    ValueTask<Int32> CountAsync(CancellationToken cancellationToken = new());

    /// <summary>
    /// Retrieves the count of <see cref="BaseEvent"/>s under an aggregate in the store, asynchronously with cancellation support via <paramref name="cancellationToken"/>
    /// </summary>
    /// <param name="aggregateId">The aggregate to count events under</param>
    /// <param name="cancellationToken"></param>
    /// <returns><see cref="ValueTask{TResult}"/> of the count</returns>
    ValueTask<Int32> CountForAggregateAsync(Guid aggregateId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Appends an event to an aggregate in the stream
    /// </summary>
    /// <param name="entity">The event to add</param>
    /// <param name="cancellationToken"></param>
    /// <returns><c>True</c> when the event is added successfully, <c>False</c> otherwise</returns>
    ValueTask<bool> AddAsync(BaseEvent entity, CancellationToken cancellationToken = new());

    /// <summary>
    /// Appends multiple entities to the event stream for a particular aggregate
    /// </summary>
    /// <param name="entities">The events to add to the stream</param>
    /// <param name="cancellationToken"></param>
    /// <returns><c>True</c> when events are added, <c>False</c> otherwise</returns>
    ValueTask<bool> AddManyAsync(IEnumerable<BaseEvent> entities, CancellationToken cancellationToken = new());
}
