namespace TheOmenDen.EventStore.Aggregates;
#nullable disable
public class SerializedAggregate
{
    public String Class { get; set; }

    public DateTimeOffset? ExpiresAt { get; set; }

    public Guid Id { get; set; }

    public string Type { get; set; }

    public Guid TenantIdentifier { get; set; }
}