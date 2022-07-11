
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
        var isInTransaction = StartTransaction(nameof(_changes));
        try
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
        finally
        {
            if (isInTransaction)
            {
                EndTransaction(nameof(_changes));
            }
        }
    }

    public IAsyncEnumerable<IEvent> FlushUncommittedChangesAsync(CancellationToken cancellationToken = default)
    {
        var isInTransaction = StartTransaction(nameof(_changes));

        try
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

            return changes.ToAsyncEnumerable();
        }
        finally
        {
            if (isInTransaction)
            {
                EndTransaction(nameof(_changes));
            }
        }
    }

    public IEvent[] GetUncommittedChanges()
    {
        var isInTransaction = StartTransaction(nameof(_changes));
        try
        {
            return _changes.ToArray();
        }
        finally
        {
            if (isInTransaction)
            {
                EndTransaction(nameof(_changes));
            }
        }
    }

    public IAsyncEnumerable<IEvent> GetUncommittedChangesAsync(CancellationToken cancellationToken = default)
    {
        var isInTransaction = StartTransaction(nameof(_changes));
        try
        {
            return _changes.ToAsyncEnumerable();
        }
        finally
        {
            if (isInTransaction)
            {
                EndTransaction(nameof(_changes));
            }
        }
    }

    public void Rehydrate(IEnumerable<IEvent> history)
    {
        var isInTransaction = StartTransaction(nameof(_changes));
        try
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
        finally

        {
            if (isInTransaction)
            {
                EndTransaction(nameof(_changes));
            }
        }
    }

    public Task RehydrateAsync(IEnumerable<IEvent> history, CancellationToken cancellationToken = new CancellationToken())
    {
        var isInTransaction = StartTransaction(nameof(TState));
        try
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
        finally
        {
            if (isInTransaction)
            {
                EndTransaction(nameof(TState));
            }
        }

        return Task.CompletedTask;
    }

    protected void Apply<T>(T changeEvent, Action<T> applyAction) where T : IEvent
    {
        var isInTransaction = StartTransaction(nameof(_changes));
        try
        {
            applyAction(changeEvent);
            _changes.Add(changeEvent);
        }
        finally
        {
            if (isInTransaction)
            {
                EndTransaction(nameof(_changes));
            }
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

    private static bool StartTransaction(String key)
    {
        try
        {
            var transactionLock = LockInstance<TState>.Create(key);

            transactionLock.Wait();

            return true;
        }
        catch
        {
            return false;
        }
    }

    private static void EndTransaction(String key)
    {
        var transactionLock = LockInstance<TState>.Get(key);

        transactionLock.Release();
    }
}

