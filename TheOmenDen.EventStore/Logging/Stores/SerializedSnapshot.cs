using System.ComponentModel.DataAnnotations.Schema;

namespace TheOmenDen.EventStore.Logging.Stores;
public class SerializedSnapshot
{
    public Guid Id { get; set; }

    public Guid AggregateId { get; set; }

    public Int32 AggregateMajorVersion { get; set; }

    public Int32 AggregateMinorVersion { get; set; }

    [NotMapped]
    public AggregateState AggregateState { get; set; }
}

