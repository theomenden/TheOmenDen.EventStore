using TheOmenDen.EventStore.Events;

namespace TheOmenDen.EventStore.Interfaces;

public interface IEventSubscriber<in TTriggerEvent, out TResponse>
    where TTriggerEvent : IEvent<TResponse>
{
    TResponse Subscribe(TTriggerEvent @event);
}

public interface IAsyncEventSubscriber<in TTriggerEvent, TResponse>
    where TTriggerEvent : IEvent<TResponse>
{
    ValueTask<TResponse> SubscribeAsync(TTriggerEvent @event, CancellationToken cancellationToken = new());
}

