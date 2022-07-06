namespace TheOmenDen.EventStore.Interfaces;
/// <summary>
/// 
/// </summary>
/// <typeparam name="TSubscribedEvent"></typeparam>
/// <typeparam name="TResponse"></typeparam>
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
