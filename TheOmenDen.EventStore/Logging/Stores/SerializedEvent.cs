namespace TheOmenDen.EventStore.Logging.Stores;
#nullable disable
public class SerializedEvent: IEvent
{
    public Guid Id { get; set; }
    public int MajorVersion { get; set; }
    public int MinorVersion { get; set; }
    public Guid IdentityTenant { get; set; }
    public Guid IdentityUser { get; set; }
    public DateTimeOffset EventTime { get; set; }

    public Type UnderlyingType { get; set; }

    public string EventType { get; set; }

    public string EventData { get; set; }

    public SerializedEvent()
    {
        EventTime = DateTimeOffset.UtcNow;
    }
}
