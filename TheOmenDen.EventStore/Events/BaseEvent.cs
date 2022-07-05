namespace TheOmenDen.EventStore.Events;

public abstract class BaseEvent: IEquatable<BaseEvent>, IComparable<BaseEvent>
{
    protected BaseEvent()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.Now;
    }

    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public int MajorVersion { get; set; }

    public int MinorVersion { get; set; }

    public string Data { get; set; }

    public Type AggregateType { get; set; }

    public Guid AggregateId { get; set; }

    public string Aggregate { get; set; }

    public override string ToString()
    {
        return $"{Aggregate}: {AggregateType} {MajorVersion}.{MinorVersion}";
    }

    public bool Equals(BaseEvent other)
    {
        return other is not null &&
               Id == other.Id
               || (
                   AggregateId == other.AggregateId
                   && MinorVersion == other.MinorVersion
                   && MajorVersion == other.MajorVersion
               );
    }

    public override bool Equals(object obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        return obj is BaseEvent other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, CreatedAt);
    }

    public int CompareTo(BaseEvent other)
    {
        return MinorVersion.CompareTo(other.MinorVersion)
               + MajorVersion.CompareTo(other.MajorVersion)
               + CreatedAt.CompareTo(other.CreatedAt);
    }
}

public abstract class BaseEvent<T> : BaseEvent, IEvent<T> 
{
    protected BaseEvent()
        :base()
    {
        AggregateType = typeof(T);
    }
}
