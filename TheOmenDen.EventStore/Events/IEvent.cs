namespace TheOmenDen.EventStore.Events;
/// <summary>
/// Marker interface for eventual subscribing
/// </summary>
public interface IEvent: IEntity
{
}
public interface IEvent<TResponse>: IEvent
{
}

