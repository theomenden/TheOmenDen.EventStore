
namespace TheOmenDen.EventStore.Locking;
internal sealed class Lockpool : ConcurrentDictionary<String, SemaphoreSlim>
{
    private static readonly Lazy<Lockpool> LazyPool = new(CreatePool);

    internal static Lockpool Instance  => LazyPool.Value;

    private Lockpool(ConcurrentDictionary<String, SemaphoreSlim> dictionary)
        : base(dictionary)
    { }

    private static Lockpool CreatePool() => new (new ());
  
}
