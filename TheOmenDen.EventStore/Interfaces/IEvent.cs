
namespace TheOmenDen.EventStore.Interfaces;
/// <summary>
/// Marker interface for eventual subscribing
/// </summary>
public interface IEvent
{
    Guid Id { get; set; }

    Int32 MajorVersion { get; set; }

    Int32 MinorVersion { get; set; }

    Guid IdentityTenant { get; set; }

    Guid IdentityUser { get; set; }

    DateTimeOffset EventTime { get; set; }

    Type UnderlyingType { get; set; }
}
