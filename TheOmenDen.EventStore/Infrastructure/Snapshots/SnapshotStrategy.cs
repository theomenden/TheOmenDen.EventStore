namespace TheOmenDen.EventStore.Infrastructure.Snapshots;
public class SnapshotStrategy: ISnapshotStrategy
{
    private readonly Int32 _interval;

    public SnapshotStrategy(int interval)
    {
        _interval = interval;
    }

    public bool ShouldTakeSnapshot(IAggregateRoot aggregate)
    {
        var i = aggregate.MajorVersion;

        for (var j = 0; j < aggregate.GetUncommittedChanges().Length; j++)
        {
            if (++i % _interval ==0 && i != 0)
            {
                return true;
            }
        }

        return false;
    }

    public ValueTask<bool> ShouldTakeSnapshotAsync(IAggregateRoot aggregate,
        CancellationToken cancellationToken = default)
    {
        var i = aggregate.MajorVersion;

        for (var j = 0; j < aggregate.GetUncommittedChanges().Length; j++)
        {
            if (++i % _interval == 0 && i != 0)
            {
                return ValueTask.FromResult(true);
            }
        }

        return ValueTask.FromResult(false);
    }
}
