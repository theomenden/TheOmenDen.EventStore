namespace TheOmenDen.EventStore.Interfaces;

public interface IEventBroker
{
}

public interface IEventBroker<in TEvent> : IEventBroker
where TEvent : BaseEvent
{
    Task AddEventToBufferAsync(TEvent @event, CancellationToken cancellationToken = new ());

    ValueTask<BaseEvent> PopEventFromBufferAsync(CancellationToken cancellationToken = new());

    Task FlushEventsFromBufferAsync(CancellationToken cancellationToken = new());
}