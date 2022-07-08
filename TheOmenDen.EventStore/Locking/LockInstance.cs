namespace TheOmenDen.EventStore.Locking;
#nullable disable


public static class LockInstance<T> 
    where T : class
{
    private static readonly Lockpool _locks = Lockpool.Instance;
    private static readonly object _cacheLock = new ();

    public static SemaphoreSlim Get(String key)
    {
        lock (_cacheLock)
        {
            return _locks.ContainsKey(GetLockKey(key))
                ? _locks[GetLockKey(key)] 
                : null;
        }
    }

    public static SemaphoreSlim Create(String key)
    {
        lock(_cacheLock)
        {
            if(!_locks.ContainsKey(GetLockKey(key)))
            {
                _locks.TryAdd(GetLockKey(key), new SemaphoreSlim(1, 1));
            }

            return _locks[GetLockKey(key)];
        }
    }

    public static void Remove(String key)
    {
        lock(_cacheLock)
        {
            if(_locks.ContainsKey(GetLockKey(key)) &&
                _locks.TryRemove(GetLockKey(key), out var removedSemaphore))
            {
                removedSemaphore.Release();
            }
        }
    }

    private static String GetLockKey(String key) => $"{typeof(T).Name}_{key}";

}