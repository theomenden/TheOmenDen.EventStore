namespace TheOmenDen.EventStore.Broker;
internal sealed class EventBroker<TEvent>: IEventBroker<TEvent>
where TEvent : BaseEvent
{
    private readonly ConcurrentQueue<BaseEvent> _events = new ();

    private readonly IEventStream _eventStream;

    public EventBroker(IEventStream eventStream)
    {
        _eventStream = eventStream;
    }

    public Task AddEventToBufferAsync(TEvent @event, CancellationToken cancellationToken = default)
    {
        _events.Enqueue(@event);

        return Task.CompletedTask;
    }

    public async ValueTask<BaseEvent> PopEventFromBufferAsync(CancellationToken cancellationToken = default)
    {
        if (_events.TryDequeue(out var eventToPush))
        {
            await _eventStream.AddAsync(eventToPush, cancellationToken);
        }

        return eventToPush ?? default;
    }

    public async Task FlushEventsFromBufferAsync(CancellationToken cancellationToken = default)
    {
        await foreach (var @event in _events.ToAsyncEnumerable().WithCancellation(cancellationToken))
        {
            await _eventStream.AddAsync(@event, cancellationToken);
        }
    }
}

