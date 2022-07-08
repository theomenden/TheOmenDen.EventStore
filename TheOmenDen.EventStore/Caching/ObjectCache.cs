namespace TheOmenDen.EventStore.Caching;
public class ObjectCache : Cache<String, Object>
{
    private static readonly Lazy<ObjectCache> Lazy = new();

    public static ObjectCache GlobalInstance => Lazy.Value;
}