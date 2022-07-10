namespace TheOmenDen.EventStore.Logging.Stores;
#nullable disable
public class SerializedAggregate
{
    public string Class { get; set; }

    public DateTimeOffset? ExpiresAt { get; set; }

    public Guid Id { get; set; }

    public string Type { get; set; }

    public Guid TenantIdentifier { get; set; }
}