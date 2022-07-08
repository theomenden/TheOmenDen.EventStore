namespace TheOmenDen.EventStore.Infrastructure;
public class EventQueue: IEventQueue
{
    private readonly Dictionary<String, List<Action<IEvent>>> _subscribers;

    private readonly Dictionary<EventOverrideKey, Action<IEvent>> _overriders;

    public EventQueue()
    {
        _subscribers = new Dictionary<string, List<Action<IEvent>>>();
        _overriders = new Dictionary<EventOverrideKey, Action<IEvent>>();
    }

    public void Publish(IEvent @event)
    {
        var eventName = @event.GetType().GetClassName();

        var key = new EventOverrideKey
        {
            EventName = eventName,
            IdentityTenant = @event.IdentityTenant
        };

        if (_overriders.Keys.Any(k => k.EventName == key.EventName && k.IdentityTenant == @event.IdentityTenant))
        {
            var customization = _overriders
                .FirstOrDefault(kv => kv.Key.EventName == key.EventName
                && kv.Key.IdentityTenant == @event.IdentityTenant)
                .Value;

            customization?.Invoke(@event);

            return;
        }

        if (!_subscribers.TryGetValue(eventName, out var actions))
        {
            throw new UnhandledEventException(eventName);
        }

        foreach (var action in actions)
        {
            action.Invoke(@event);
        }
    }

    public Task PublishAsync(IEvent @event, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public void Subscribe<T>(Action<T> action) where T : IEvent
    {
        var name = typeof(T).GetClassName();

        if (!_subscribers.Any(x => x.Key == name))
            _subscribers.Add(name, new List<Action<IEvent>>());

        _subscribers[name].Add((@event) => action((T)@event));
    }

    public Task SubscribeAsync<T>(Action<T> action, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public void Override<T>(Action<T> action, Guid tenant) where T : IEvent
    {
        var key = new EventOverrideKey
        {
            EventName = typeof(T).FullName ?? nameof(T), 
            IdentityTenant = tenant
        };

        if (_overriders.Any(x => x.Key.EventName == key.EventName && x.Key.IdentityTenant == key.IdentityTenant))
        {
            throw new AmbiguousCommandHandlerException(key?.EventName ?? nameof(key));
        }

        _overriders.Add(key, (command) => action((T)command));
    }

    public Task OverrideAsync<T>(Action<T> action, Guid tenant, CancellationToken cancellationToken = new CancellationToken()) where T : IEvent
    {
        throw new NotImplementedException();
    }
}

