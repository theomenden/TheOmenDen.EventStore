namespace TheOmenDen.EventStore.Interfaces;
/// <summary>
/// Defines a simple messaging queue to allow for event publishing, subscription, and overriding
/// </summary>
public interface IEventQueue
{
    void Publish(IEvent @event);

    Task PublishAsync(IEvent @event, CancellationToken cancellationToken = new());

    void Subscribe<T>(Action<T> action) where T : IEvent;

    Task SubscribeAsync<T>(Action<T> action, CancellationToken cancellationToken = new());

    void Override<T>(Action<T> action, Guid tenant) where T : IEvent;

    Task OverrideAsync<T>(Action<T> action, Guid tenant, CancellationToken cancellationToken = new()) where T : IEvent;
}