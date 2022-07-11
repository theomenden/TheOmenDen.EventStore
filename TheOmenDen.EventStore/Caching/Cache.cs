namespace TheOmenDen.EventStore.Caching;
#nullable disable
public class Cache<TKey, TItem> : ICache<TKey, TItem>
{
    #region Private Fields
    private bool _disposed;
    private readonly Dictionary<TKey, TItem> _cache = new();
    private readonly Dictionary<TKey, Timer> _timers = new();
    #endregion
    protected Cache() { }
    #region Destruction Methods
    public void Clear()
    {
        var isInTransaction = StartTransaction(nameof(TKey));

        try
        {
            try
            {
                foreach (var timer in _timers.Values)
                {
                    timer.Dispose();
                }
            }
            catch
            {
                // ignored
            }

            _timers.Clear();
            _cache.Clear();
        }
        finally
        {
            if (isInTransaction)
            {
                EndTransaction(nameof(TKey));
            }
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        if (!disposing)
        {
            return;
        }
        Clear();

        LockInstance<TItem>.Remove(nameof(TKey));
    }
    #endregion
    #region Implementations
    public TItem this[TKey key] => Get(key);

    public TItem Get(TKey key)
    {
        if (_disposed)
        {
            return default;
        }

        var isInTransaction = StartTransaction(nameof(TKey));

        try
        {
            return _cache.TryGetValue(key, out var value)
                ? value
                : default;
        }
        finally
        {
            if (isInTransaction)
            {
                EndTransaction(nameof(TKey));
            }
        }
    }

    public (bool, TItem) TryGet(TKey key)
    {
        if (_disposed)
        {
            return new(false, default);
        }

        var isInTransaction = StartTransaction(nameof(TKey));

        try
        {

            return new(_cache.TryGetValue(key, out var value), value);
        }
        finally
        {
            if (isInTransaction)
            {
                EndTransaction(nameof(TKey));
            }
        }
    }

    public bool Exists(TKey key)
    {
        if (_disposed)
        {
            return false;
        }

        var isInTransaction = StartTransaction(nameof(TKey));


        try
        {
            return _cache.ContainsKey(key);
        }
        finally
        {
            if (isInTransaction)
            {
                EndTransaction(nameof(TKey));
            }
        }
    }

    public void Add(TKey key, TItem value)
    {
        Add(key, value, Timeout.Infinite);
    }

    public void Add(TKey key, TItem value, int timeout, bool restartTimer = false)
    {
        if (_disposed)
        {
            return;
        }

        if (timeout == Timeout.Infinite && timeout < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(timeout));
        }

        var isInTransaction = StartTransaction(nameof(TKey));

        try
        {
            CheckTimer(key, timeout, restartTimer);

            if (!_cache.ContainsKey(key))
            {
                _cache.Add(key, value);

                return;
            }

            _cache[key] = value;
        }
        finally
        {
            if (isInTransaction)
            {
                EndTransaction(nameof(TKey));
            }
        }
    }

    public void Remove(TKey key)
    {
        if (_disposed)
        {
            return;
        }

        var isInTransaction = StartTransaction(nameof(TKey));

        try
        {
            if (!_cache.ContainsKey(key))
            {
                return;
            }

            AttemptTimerDisposal(key);
        }
        finally
        {
            if (isInTransaction)
            {
                EndTransaction(nameof(TKey));
            }
        }
    }

    public void Remove(Predicate<TKey> pattern)
    {
        if (_disposed)
        {
            return;
        }

        var isInTransaction = StartTransaction(nameof(TKey));

        try
        {
            var removers = _cache.Keys
                .Where(k => pattern(k))
                .Select(k => k)
                .ToList();

            foreach (var workKey in removers)
            {
                AttemptTimerDisposal(workKey);
            }
        }
        finally
        {
            if (isInTransaction)
            {
                EndTransaction(nameof(TKey));
            }
        }
    }

    public bool StartTransaction(String key)
    {
        try
        {
            var transactionLock = LockInstance<TKey>.Create(key);

            transactionLock.Wait();

            return true;
        }
        catch
        {
            return false;
        }
    }

    public void EndTransaction(String key)
    {
        var transactionLock = LockInstance<TKey>.Get(key);

        transactionLock.Release();
    }
    #endregion
    #region Private Methods
    private void CheckTimer(TKey key, Int32 cacheTimeout, Boolean restartTimerIfExists)
    {
        if (_timers.TryGetValue(key, out var timer)
            && restartTimerIfExists)
        {
            timer.Change(
                cacheTimeout == Timeout.Infinite ? Timeout.Infinite : cacheTimeout * 1000
                , Timeout.Infinite);

            return;
        }

        _timers.Add(key, new(
            RemoveByTimer,
            key,
            (cacheTimeout == Timeout.Infinite ? Timeout.Infinite : cacheTimeout * 1000),
            Timeout.Infinite
            ));
    }

    private void RemoveByTimer(object state)
    {
        Remove((TKey)state);
    }

    private void AttemptTimerDisposal(TKey key)
    {
        try
        {
            _timers[key].Dispose();
        }
        catch
        {
            // ignored
        }

        _timers.Remove(key);
        _cache.Remove(key);
    }
    #endregion
}