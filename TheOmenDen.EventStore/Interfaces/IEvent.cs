namespace TheOmenDen.EventStore.Interfaces;
/// <summary>
/// Marker interface for eventual subscribing
/// </summary>
public interface IEvent
{
    Guid Id { get; set; }

    int MajorVersion { get; set; }

    int MinorVersion { get; set; }

    Guid IdentityTenant { get; set; }

    Guid IdentityUser { get; set; }

    DateTimeOffset EventTime { get; set; }
}
