namespace TheOmenDen.EventStore.Locking;
#nullable disable
/// <summary>
/// Creates an instance of a <see cref="Semaphore"/> to lock down operations over a sequence
/// </summary>
/// <typeparam name="T">The underlying type requiring the <see cref="Semaphore"/></typeparam>
public static class LockInstance<T> 
{
    private static readonly LockPool _locks = LockPool.Instance;
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
                _locks.TryAdd(GetLockKey(key), new (1, 1));
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