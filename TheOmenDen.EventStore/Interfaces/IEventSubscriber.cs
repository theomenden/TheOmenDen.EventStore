namespace TheOmenDen.EventStore.Interfaces;

public interface IEventSubscriber<in TTriggerEvent, out TResponse>
    where TTriggerEvent : IEvent<TResponse>
{
    TResponse Subscribe(TTriggerEvent @event);

    void Unsubscribe(Guid eventId);
}

public interface IAsyncEventSubscriber<out TTriggerEvent, TResponse>
    where TTriggerEvent : IEvent<TResponse>
{
    ValueTask<TResponse> SubscribeAsync(Action<TTriggerEvent> onSubscription, CancellationToken cancellationToken = new());

    ValueTask<TResponse> SubscribeAsync(Action<bool> onEventSubscription, CancellationToken cancellationToken = new());

    Task UnsubscribeAsync(Action<BaseEvent, CancellationToken> onEventUnsubscribe, CancellationToken cancellationToken = new());
}

