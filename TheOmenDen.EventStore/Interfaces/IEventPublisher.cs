using TheOmenDen.EventStore.Events;

namespace TheOmenDen.EventStore.Interfaces;
public interface IEventPublisher<in TSubscribedEvent, out TResponse>
    where TSubscribedEvent : IEvent<TResponse>
{
    TResponse Publish(TSubscribedEvent @event);
}

public interface IAsyncEventPublisher<in TSubscribedEvent, TResponse>
    where TSubscribedEvent : IEvent<TResponse>
{
    ValueTask<TResponse> PublishAsync(TSubscribedEvent @event);
}
