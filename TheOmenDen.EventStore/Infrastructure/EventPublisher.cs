namespace TheOmenDen.EventStore.Infrastructure;
public abstract class EventPublisher<TTRiggerEvent, TResponse>: IEventPublisher<TTRiggerEvent, TResponse> 
    where TTRiggerEvent : BaseEvent, IEvent<TResponse>
{
    protected EventPublisher(IEventStream events)
    {
        Events = events;
    }

    protected IEventStream Events { get; }

    public abstract TResponse Publish(TTRiggerEvent @event);
}

public abstract class AsyncEventPublisher<TTriggerEvent, TResponse> : IAsyncEventPublisher<TTriggerEvent, TResponse>
    where TTriggerEvent : BaseEvent, IEvent<TResponse>
{
    protected AsyncEventPublisher(IEventBroker<TTriggerEvent> broker)
    {
        Broker = broker;
    }

    protected IEventBroker<TTriggerEvent> Broker { get; }

    public abstract ValueTask<TResponse> PublishAsync(TTriggerEvent @event);
}