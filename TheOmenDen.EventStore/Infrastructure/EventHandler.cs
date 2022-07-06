namespace TheOmenDen.EventStore.Infrastructure;

public abstract class EventHandler<TTriggerEvent, TResponse> : IEventSubscriber<TTriggerEvent, TResponse>
    where TTriggerEvent : BaseEvent, IEvent<TResponse> 
{
    protected EventHandler(IEventStream eventStream)
    {
        EventStream = eventStream;
    }

    protected IEventStream EventStream { get; }

    public abstract TResponse Subscribe(TTriggerEvent @event);

    public abstract void Unsubscribe(Guid eventId);
}

public abstract class AsyncEventHandler<TTriggerEvent, TResponse> : IAsyncEventSubscriber<TTriggerEvent, TResponse>
    where TTriggerEvent : BaseEvent, IEvent<TResponse>
{
    protected AsyncEventHandler(IEventBroker<TTriggerEvent> eventBroker)
    {
        EventBroker = eventBroker;
    }

    protected IEventBroker<TTriggerEvent> EventBroker { get; }

    public abstract ValueTask<TResponse> SubscribeAsync(TTriggerEvent @event, CancellationToken cancellationToken = default);

    public abstract ValueTask<TResponse> SubscribeAsync(Action<BaseEvent, CancellationToken> onEventSubscription);

    public abstract ValueTask<TResponse> SubscribeAsync(Action<TTriggerEvent> onSubscription, CancellationToken cancellationToken = default);
    
    public abstract ValueTask<TResponse> SubscribeAsync(Action<bool> onEventSubscription, CancellationToken cancellationToken = default);

    public abstract Task UnsubscribeAsync(Action<BaseEvent, CancellationToken> onEventUnsubscribe, CancellationToken cancellationToken = default);
}