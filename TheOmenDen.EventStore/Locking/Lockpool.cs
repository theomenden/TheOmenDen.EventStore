namespace TheOmenDen.EventStore.Locking;
internal sealed class LockPool : ConcurrentDictionary<String, SemaphoreSlim>
{
    private static readonly Lazy<LockPool> LazyPool = new(CreatePool);

    internal static LockPool Instance  => LazyPool.Value;

    private LockPool(ConcurrentDictionary<String, SemaphoreSlim> dictionary)
        : base(dictionary)
    { }

    private static LockPool CreatePool() => new (new ());
  
}
