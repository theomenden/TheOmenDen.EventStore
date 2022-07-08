namespace TheOmenDen.EventStore.Aggregates;

public interface IAggregateRoot
{
    Guid Id { get; set; }

    int MajorVersion { get; set; }

    int MinorVersion { get; set; }

    AggregateState State { get; set; }

    IEvent[] FlushUncommittedChanges();

    ValueTask<IEvent[]> FlushUncommittedChangesAsync(CancellationToken cancellationToken = new());

    IEvent[] GetUncommittedChanges();

    ValueTask<IEvent[]> GetUncommittedChangesAsync(CancellationToken cancellationToken = new());

    void Rehydrate(IEnumerable<IEvent> history);

    Task RehydrateAsync(IEnumerable<IEvent> history, CancellationToken cancellationToken = new());
}

