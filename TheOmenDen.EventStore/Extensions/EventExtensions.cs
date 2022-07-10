namespace TheOmenDen.EventStore.Extensions;
public static class EventExtensions
{
    public static IEvent Deserialize(this SerializedEvent serializedEvent)
    {
        var data = JsonSerializer.Deserialize<IEvent>(serializedEvent.EventData);

        data.Id = serializedEvent.Id;
        data.MajorVersion = serializedEvent.MajorVersion;
        data.MinorVersion = serializedEvent.MinorVersion;
        data.IdentityTenant = serializedEvent.IdentityTenant;
        data.IdentityUser = serializedEvent.IdentityUser;
        data.UnderlyingType = serializedEvent.UnderlyingType;

        return data;
    }

    public static SerializedEvent Serialize(this IEvent @event, Guid aggregateId, Int32 minorVersion, Int32 majorVersion, Guid tenantId,
        Guid userId)
    {
        var data = JsonSerializer.Serialize(@event);

        var serialized = new SerializedEvent
        {
            Id = aggregateId,
            MajorVersion = majorVersion,
            MinorVersion = minorVersion,
            EventTime = @event.EventTime,
            UnderlyingType = @event.GetType(),
            EventData = data,
            IdentityTenant = Guid.Empty == tenantId ? tenantId : @event.IdentityTenant,
            IdentityUser = Guid.Empty == userId ? userId : @event.IdentityUser,
        };

        @event.IdentityTenant = serialized.IdentityTenant;
        @event.IdentityUser = serialized.IdentityUser;

        return serialized;
    }
}

