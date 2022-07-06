namespace TheOmenDen.EventStore.Events;
/// <summary>
/// Marker interface for eventual subscribing
/// </summary>
public interface IEvent: IEntity
{
}

/// <summary>
/// Marker interface for subscribing to events that return a response
/// </summary>
/// <typeparam name="TResponse">The outgoing response</typeparam>
public interface IEvent<TResponse>: IEvent
{
}

