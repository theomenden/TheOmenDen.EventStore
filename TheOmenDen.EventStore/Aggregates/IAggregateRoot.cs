namespace TheOmenDen.EventStore.Aggregates;

public interface IAggregateRoot
{
    Guid Id { get; set; }

    Int32 MajorVersion { get; set; }

    Int32 MinorVersion { get; set; }

    AggregateState State { get; set; }

    IEvent[] FlushUncommittedChanges();

    IAsyncEnumerable<IEvent> FlushUncommittedChangesAsync(CancellationToken cancellationToken = new());

    IEvent[] GetUncommittedChanges();

    IAsyncEnumerable<IEvent> GetUncommittedChangesAsync(CancellationToken cancellationToken = new());

    void Rehydrate(IEnumerable<IEvent> history);

    Task RehydrateAsync(IEnumerable<IEvent> history, CancellationToken cancellationToken = new());
}

