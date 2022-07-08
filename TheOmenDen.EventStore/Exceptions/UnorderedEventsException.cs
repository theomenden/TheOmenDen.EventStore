namespace TheOmenDen.EventStore.Exceptions;
internal class UnorderedEventsException: Exception
{
    public UnorderedEventsException(Guid aggregate)
    :base($"The events for this aggregate are not in the expected order ({aggregate}).")
    {
    }
}
