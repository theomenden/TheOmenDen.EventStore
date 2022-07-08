namespace TheOmenDen.EventStore.Aggregates;
#nullable disable
public abstract class AggregateRoot<TState> : IAggregateRoot
where TState : AggregateState, new()
{
    private readonly List<IEvent> _changes = new(20);

    public Guid Id { get; set; }
    
    public int MajorVersion { get; set; }
    
    public int MinorVersion { get; set; }

    public TState State { get; set; }

    AggregateState IAggregateRoot.State
    {
        get => State;
        set => State = (TState)value ?? throw new ArgumentNullException(nameof(value));
    }

    public IEvent[] FlushUncommittedChanges()
    {
        lock (_changes)
        {
            var changes = _changes.ToArray();

            var i = 0;

            foreach (var change in changes)
            {
                if (change.Id == Guid.Empty && Id == Guid.Empty)
                {
                    throw new MissingAggregateIdentifierException(GetType(), change.GetType());
                }

                if (change.Id == Guid.Empty)
                {
                    change.Id = Id;
                }

                i++;
                
                change.MinorVersion = MinorVersion + i;
                change.EventTime = DateTimeOffset.UtcNow;
            }

            MinorVersion += changes.Length;

            _changes.Clear();

            return changes;
        }
    }

    public async ValueTask<IEvent[]> FlushUncommittedChangesAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public IEvent[] GetUncommittedChanges()
    {
        lock(_changes)
        {
            return _changes.ToArray();
        }
    }

    public async ValueTask<IEvent[]> GetUncommittedChangesAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public void Rehydrate(IEnumerable<IEvent> history)
    {
        lock (_changes)
        {
            foreach (var change in history.ToArray())
            {
                if (change.MinorVersion != MinorVersion + 1)
                {
                    throw new UnorderedEventsException(change.Id);
                }

                ApplyEvent(change);

                Id = change.Id;

                MinorVersion++;
            }
        }
    }

    public async Task RehydrateAsync(IEnumerable<IEvent> history, CancellationToken cancellationToken = new CancellationToken())
    {
        foreach (var change in history.ToList().AsReadOnly())
        {
            if (change.MinorVersion != MinorVersion + 1)
            {
                throw new UnorderedEventsException(change.Id);
            }

            ApplyEvent(change);

            Id = change.Id;

            MinorVersion++;
        }
    }

    protected void Apply<T>(T changeEvent, Action<T> applyAction) where T : IEvent
    {
        lock (_changes)
        {
            applyAction(changeEvent);
            _changes.Add(changeEvent);
        }
    }

    protected virtual void ApplyEvent(IEvent change)
    {
        State.Apply(change);
    }

    protected virtual Task ApplyEventAsync(IEvent change, CancellationToken cancellationToken = new())
    {
        return State.ApplyAsync(change);
    }
}

