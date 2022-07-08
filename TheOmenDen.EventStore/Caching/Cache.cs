namespace TheOmenDen.EventStore.Caching;
#nullable disable
public class Cache<TKey, TITem>: ICache<TKey, TITem>, IDisposable
{
    #region Private Fields
    private bool _disposed;
    private readonly Dictionary<TKey, TITem> _cache = new ();
    private readonly Dictionary<TKey, Timer> _timers = new();
    private readonly ReaderWriterLockSlim _locker = new ReaderWriterLockSlim ();
    #endregion
    protected Cache(){}
    #region Destruction Methods
    public void Clear()
    {
        _locker.EnterWriteLock();

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
            _locker.ExitWriteLock();
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
        _locker.Dispose();
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
    #endregion

    public TITem this[TKey key] => Get(key);

    public TITem Get(TKey key)
    {
        if (_disposed)
        {
            return default;
        }

        _locker.EnterReadLock();

        try
        {
            return _cache.TryGetValue(key, out var value) 
                ? value 
                : default;
        }
        finally
        {
            _locker.ExitReadLock();
        }
    }

    public (bool, TITem) TryGet(TKey key)
    {
        if (_disposed)
        {
            return new (false, default);
        }

        _locker.EnterReadLock();

        try
        {

            return new(_cache.TryGetValue(key, out var value), value);
        }
        finally
        {
            _locker.ExitReadLock();
        }
    }

    public bool Exists(TKey key)
    {
        if (_disposed)
        {
            return false;
        }

        _locker.EnterReadLock();

        try
        {
            return _cache.ContainsKey(key);
        }
        finally
        {
            _locker.ExitReadLock();
        }
    }

    public void Add(TKey key, TITem value)
    {
        Add(key, value, Timeout.Infinite);
    }

    public void Add(TKey key, TITem value, int timeout, bool restartTimer = false)
    {
        if (_disposed)
        {
            return;
        }

        if (timeout == Timeout.Infinite && timeout < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(timeout));
        }

        _locker.EnterWriteLock();
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
            _locker.ExitWriteLock();
        }
    }

    public void Remove(TKey key)
    {
        if (_disposed)
        {
            return;
        }

        _locker.EnterWriteLock();

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
            _locker.ExitWriteLock();
        }
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

    public void Remove(Predicate<TKey> pattern)
    {
        if (_disposed)
        {
            return;
        }

        _locker.EnterWriteLock();

        try
        {
            var removers = _cache.Keys
                .Where(k => pattern(k))
                .Select(k => k)
                .ToList();

            foreach(var workKey in removers)
            {
                AttemptTimerDisposal(workKey);
            }
        }
        finally
        {
            _locker.ExitWriteLock();
        }
    }
}