namespace TheOmenDen.EventStore.Events;
#nullable disable
/// <summary>
/// <para>A base event class that allows for the construction and unique identifying of a particular event</para>
/// <para>Meant to be subscribed to with a compatible <see cref="IEventSubscriber{TTriggerEvent, TResponse}"/></para>
/// <para>Eventually to be published by a compatible <see cref="IEventPublisher{TSubscribedEvent, TResponse}"/></para>
/// <inheritdoc cref="IEquatable{T}"/>
/// <inheritdoc cref="IComparable{T}"/>
/// </summary>
public abstract class BaseEvent : IEquatable<BaseEvent>, IComparable<BaseEvent>
{
    protected BaseEvent()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.Now;
    }

    public Guid Id { get; init; }

    public DateTime CreatedAt { get; init; }

    public int MajorVersion { get; set; } = 1;

    public int MinorVersion { get; set; } = 0;

    public string Data { get; init; }

    public Type AggregateType { get; init; }

    public Guid AggregateId { get; init; }

    public override string ToString() => $"{AggregateType} {MajorVersion}.{MinorVersion}";

    public bool Equals(BaseEvent other) =>
        other is not null 
        && Id == other.Id
        || (
               AggregateId  == other.AggregateId
            && MinorVersion == other.MinorVersion 
            && MajorVersion == other.MajorVersion
            );


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

    public override int GetHashCode() => HashCode.Combine(Id, CreatedAt);

    public int CompareTo(BaseEvent other)
    {
        if (other is null)
        {
            return 1;
        }

        return MinorVersion.CompareTo(other.MinorVersion)
               + MajorVersion.CompareTo(other.MajorVersion)
               + CreatedAt.CompareTo(other.CreatedAt);
    }
}

/// <summary>
/// Generic wrapper for <see cref="BaseEvent"/>
/// </summary>
/// <typeparam name="T">The underlying type the event is acting upon</typeparam>
public abstract class BaseEvent<T> : BaseEvent, IEvent<T>
{
    protected BaseEvent()
        : base()
    {
        AggregateType = typeof(T);
    }
}
