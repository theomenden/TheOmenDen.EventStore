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
    protected AsyncEventHandler(IEventStream eventStream)
    {
        EventStream = eventStream;
    }

    protected IEventStream EventStream { get; }

    public abstract ValueTask<TResponse> SubscribeAsync(TTriggerEvent @event,
        CancellationToken cancellationToken = default);

    public abstract Task UnsubscribeAsync(Guid eventId);
}