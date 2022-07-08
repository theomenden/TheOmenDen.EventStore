namespace TheOmenDen.EventStore.Interfaces;
public interface ISnapshotStrategy
{
    bool ShouldTakeSnapshot(IAggregateRoot aggregate);

    ValueTask<bool> ShouldTakeSnapshotAsync(IAggregateRoot aggregate, CancellationToken cancellationToken = new());
}

