namespace TheOmenDen.EventStore.Events;

internal class EventOverrideKey
{
    public string EventName { get; set; }

    public Guid IdentityTenant { get; set; }
}
