namespace TheOmenDen.EventStore.Events;
#nullable disable
internal class EventOverrideKey
{
    public string EventName { get; set; }

    public Guid IdentityTenant { get; set; }
}
