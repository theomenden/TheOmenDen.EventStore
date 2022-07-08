namespace TheOmenDen.EventStore.Caching;

public interface ICache<TKey, TItem>: IDisposable
{
    TItem this[TKey key] { get; }
    
    TItem Get(TKey key);

    ValueTuple<bool, TItem> TryGet(TKey key);

    bool Exists(TKey key);

    void Add(TKey key, TItem value);

    void Add(TKey key, TItem value, Int32 timeout, Boolean restartTimer = false);

    void Remove(TKey key);

    void Remove(Predicate<TKey> pattern);

    void Clear();
}

