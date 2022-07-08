using TheOmenDen.EventStore.Aggregates;

namespace TheOmenDen.EventStore.Infrastructure.Snapshots;
public sealed class Snapshot
{
    public Guid AggregateIdentifier { get; set; }

    public Int32 AggregateMajorVersion { get; set; }

    public Int32 AggregateMinorVersion { get; set; }

    public AggregateState AggregateState { get; set; }
}

