namespace TheOmenDen.EventStore.Interfaces;
public interface IEventStore
{
    #region Existence Checks
    bool Exists(Guid aggregate);

    ValueTask<bool> ExistsAsync(Guid aggregate, CancellationToken cancellationToken = new());

    bool Exists(Guid aggregate, int version);

    ValueTask<bool> ExistsAsync(Guid aggregate, int version, CancellationToken cancellationToken = new());
    #endregion
    #region Get Methods
    IEnumerable<IEvent> Get(Guid aggregate, int majorVersion);

    IAsyncEnumerable<IEvent> GetAsync(Guid aggregate, int majorVersion, CancellationToken cancellationToken = new());

    IEnumerable<Guid> GetExpired(DateTimeOffset expiredAt);

    IAsyncEnumerable<Guid> GetExpiredAsync(DateTimeOffset expiredAt, CancellationToken cancellationToken = new());
    #endregion
    #region Save Methods
    void Save(IAggregateRoot aggregate, IEnumerable<IEvent> events);

    Task SaveAsync(IAggregateRoot aggregate, IEnumerable<IEvent> events, CancellationToken cancellationToken = new());
    #endregion
    #region Offline Storage Methods
    void Box(Guid aggregate);

    Task BoxAsync(Guid aggregate, CancellationToken cancellationToken = new());
    #endregion
}