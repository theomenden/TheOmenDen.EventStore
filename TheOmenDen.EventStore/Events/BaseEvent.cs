namespace TheOmenDen.EventStore.Events;
#nullable disable
/// <summary>
/// <para>A base event class that allows for the construction and unique identifying of a particular event</para>
/// <para>Meant to be subscribed to with a compatible <see cref="IEventSubscriber{TTriggerEvent, TResponse}"/></para>
/// <para>Eventually to be published by a compatible <see cref="IEventPublisher{TSubscribedEvent, TResponse}"/></para>
/// <inheritdoc cref="IEquatable{T}"/>
/// <inheritdoc cref="IComparable{T}"/>
/// </summary>
public abstract class BaseEvent : IEvent, IEquatable<BaseEvent>
{
    protected BaseEvent()
    {
        Id = Guid.NewGuid();
    }

    public Guid Id { get; set; }
    public abstract int MajorVersion { get; set; }
    public abstract int MinorVersion { get; set; }
    public abstract Guid IdentityTenant { get; set; }
    public abstract Guid IdentityUser { get; set; }
    public abstract DateTimeOffset EventTime { get; set; }
    public abstract Type UnderlyingType { get; set; }

    public bool Equals(BaseEvent other)
    {
        return other is not null
               && other.Id == Id;
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
        return HashCode.Combine(Id, EventTime);
    }
}

/// <summary>
/// Generic wrapper for <see cref="BaseEvent"/>
/// </summary>
/// <typeparam name="T">The underlying type the event is acting upon</typeparam>
public abstract class BaseEvent<T> : BaseEvent
{
    protected BaseEvent()
        : base()
    {
    }
}
