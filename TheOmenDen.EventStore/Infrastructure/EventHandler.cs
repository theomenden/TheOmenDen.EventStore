using TheOmenDen.EventStore.Events;

namespace TheOmenDen.EventStore.Infrastructure;

internal sealed class EventHandler<TTriggerEvent, TResponse> : IEventSubscriber<TTriggerEvent, TResponse>,
    IAsyncEventSubscriber<TTriggerEvent, TResponse>
    where TTriggerEvent : BaseEvent, IEvent<TResponse> 
{
    private readonly IEventStream _eventStream;

    public EventHandler(IEventStream eventStream)
    {
        _eventStream = eventStream;
    }

    public TResponse Subscribe(TTriggerEvent @event)
    {
        

        return default;
    }

    public ValueTask<TResponse> SubscribeAsync(TTriggerEvent @event, CancellationToken cancellationToken = default)
    {
        return ValueTask.FromResult<TResponse>(default);
    }
}
