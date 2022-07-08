
namespace TheOmenDen.EventStore.Locking;
internal sealed class Lockpool : ConcurrentDictionary<String, SemaphoreSlim>
{
    private static readonly Lazy<Lockpool> _lazy = new(() => CreatePool());

    internal static Lockpool Instance { get { return _lazy.Value; } }

    private Lockpool(ConcurrentDictionary<String, SemaphoreSlim> dictionary)
        : base(dictionary)
    { }

    private static Lockpool CreatePool() => new (new ());
  
}
