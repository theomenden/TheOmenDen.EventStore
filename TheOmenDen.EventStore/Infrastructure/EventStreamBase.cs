using Microsoft.Extensions.Logging;

namespace TheOmenDen.EventStore.Infrastructure;

internal sealed class EventStreamBase : IEventStream
{
    private readonly ConcurrentDictionary<Guid, List<BaseEvent>> _events;

    private readonly ILogger<EventStreamBase> _logger;

    #region Constructors
    public EventStreamBase(ILogger<EventStreamBase> logger)
    {
        _logger = logger;

        _events = new ConcurrentDictionary<Guid, List<BaseEvent>>();
    }
    #endregion
    #region Exposed Properties
    public Int32 Count => _events.Count;

    public ValueTask<Int32> CountAsync(CancellationToken cancellationToken = default)
    {
        return cancellationToken.IsCancellationRequested
            ? ValueTask.FromCanceled<Int32>(cancellationToken)
            : ValueTask.FromResult(Count);
    }

    public Int32 CountForAggregate(Guid aggregateId) => _events.Count(e => e.Key == aggregateId);

    public ValueTask<Int32> CountForAggregateAsync(Guid aggregateId, CancellationToken cancellationToken = default)
    {
        return cancellationToken.IsCancellationRequested 
            ? ValueTask.FromCanceled<Int32>(cancellationToken) 
            : ValueTask.FromResult(CountForAggregate(aggregateId));
    }
    #endregion
    #region IEnumerable Implementation
    public IEnumerator<BaseEvent> GetEnumerator()
    {
        var enumerator = GetAll().GetEnumerator();

        return enumerator;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    #endregion
    #region AsyncEnumerable Implementation
    public IAsyncEnumerator<KeyValuePair<Guid, List<BaseEvent>>> GetAsyncEnumerator(CancellationToken cancellationToken = new CancellationToken())
    {
        var enumerator = GetAllAsyncStream(cancellationToken).GetAsyncEnumerator(cancellationToken);

        return enumerator;
    }
    #endregion
    #region Appending Methods
    public ValueTask<bool> AddAsync(BaseEvent entity, CancellationToken cancellationToken = new CancellationToken())
    {
        try
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return ValueTask.FromCanceled<bool>(cancellationToken);
            }

            var valueToAdd = new List<BaseEvent> { entity };

            _events.AddOrUpdate(
                entity.AggregateId,
                valueToAdd,
                (aggregateId, existingEvents) =>
                {
                    if (existingEvents.Contains(entity))
                    {
                        throw new ArgumentException("Cannot insert duplicate events");
                    }

                    existingEvents.AddRange(valueToAdd);

                    return existingEvents;
                });

            return ValueTask.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError("Exception thrown while attempting to add the event {@Exception}", ex);
            return ValueTask.FromResult(false);
        }
    }

    public ValueTask<bool> AddManyAsync(IEnumerable<BaseEvent> entities, CancellationToken cancellationToken = new CancellationToken())
    {
        try
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return ValueTask.FromCanceled<bool>(cancellationToken);
            }

            var eventsToAdd = entities.ToList();

            var aggregateId = eventsToAdd.First().AggregateId;

            _events.AddOrUpdate(
                aggregateId,
                eventsToAdd.ToList(),
                (_, existingEvents) =>
                {
                    if (existingEvents.Any(eventsToAdd.Contains))
                    {
                        throw new ArgumentException("Cannot insert duplicate events");
                    }

                    existingEvents.AddRange(eventsToAdd);

                    return existingEvents;
                });

            return ValueTask.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError("Exception thrown while attempting to add the event {@Exception}", ex);
            return ValueTask.FromResult(false);
        }
    }
    #endregion
    #region Streaming Methods
    private IAsyncEnumerable<KeyValuePair<Guid, List<BaseEvent>>> GetAllAsyncStream(CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            cancellationToken.ThrowIfCancellationRequested();
        }

        return _events.ToAsyncEnumerable();
    }

    public IAsyncEnumerable<BaseEvent> GetAllAsAsyncStream(CancellationToken cancellationToken = new())
    {
        return _events
            .SelectMany(e => e.Value)
            .ToAsyncEnumerable();
    }

    public IAsyncEnumerable<BaseEvent> GetAllThatMatchAsAsyncStream(Expression<Func<BaseEvent, bool>> predicate,
        CancellationToken cancellationToken = new())
    {
        return _events
            .SelectMany(e => e.Value)
            .ToAsyncEnumerable()
            .Where(predicate.Compile());
    }
    #endregion
    #region Standard Retrieval Methods
    private IEnumerable<BaseEvent> GetAll()
    {
        return _events.Values
            .SelectMany(e => e)
            .OrderBy(e => e.Aggregate);
    }

    public ValueTask<IEnumerable<BaseEvent>> GetAllAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        return ValueTask.FromResult(GetAll());
    }

    public ValueTask<IEnumerable<BaseEvent>> GetAllThatMatchAsync(Expression<Func<BaseEvent, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
    {
        var eventsThatMatch = GetAll()
            .AsQueryable()
            .Where(predicate)
            .AsEnumerable();

        return ValueTask.FromResult(eventsThatMatch);
    }
    public ValueTask<BaseEvent> WithIdAsync(Guid entityId, CancellationToken cancellationToken = new CancellationToken())
    {
        var eventRetrieved = GetAll().Single(e => e.Id == entityId);

        return ValueTask.FromResult(eventRetrieved);
    }
    #endregion
}