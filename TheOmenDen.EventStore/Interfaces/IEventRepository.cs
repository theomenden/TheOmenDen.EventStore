namespace TheOmenDen.EventStore.Interfaces;
/// <summary>
/// Contains methods relevant to the storage and retrieval of <see cref="IAggregateRoot"/>s and <see cref="IEvent"/>s
/// </summary>
public interface IEventRepository
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="id"></param>
    /// <returns></returns>
    T Get<T>(Guid id) where T : IAggregateRoot;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    ValueTask<T> GetAsync<T>(Guid id, CancellationToken cancellationToken = new()) where T : IAggregateRoot;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="aggregate"></param>
    /// <param name="version"></param>
    /// <returns></returns>
    IEvent[] Save<T>(T aggregate, int? version = null) where T : IAggregateRoot;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="aggregate"></param>
    /// <param name="version"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    ValueTask<IEvent[]> SaveAsync<T>(T aggregate, int? version = null, CancellationToken cancellationToken = new())
        where T : IAggregateRoot;

    /// <summary>
    /// Removes an aggregate from an "online" storage system and preserves it to a local storage system (file by default)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="aggregate"></param>
    void Box<T>(T aggregate) where T : IAggregateRoot;

    /// <summary>
    /// Removes an aggregate from an "online" storage system and preserves it to a local storage system (file by default)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="aggregate"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task BoxAsync<T>(T aggregate, CancellationToken cancellationToken = new()) where T : IAggregateRoot;

    /// <summary>
    /// Removes an aggregate from an "offline" storage system and returns the most recent state
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="aggregate"></param>
    /// <returns></returns>
    T Unbox<T>(Guid aggregate) where T : IAggregateRoot;

    /// <summary>
    /// Removes an aggregate from an "offline" storage system and returns the most recent state
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="aggregate"></param>
    /// <returns></returns>
    ValueTask<T> UnboxAsync<T>(Guid aggregate) where T : IAggregateRoot;
}

