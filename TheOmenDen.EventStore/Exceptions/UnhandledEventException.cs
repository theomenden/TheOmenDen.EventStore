namespace TheOmenDen.EventStore.Exceptions;

internal class UnhandledEventException: Exception
{
    public UnhandledEventException(String name)
        : base($"You must register at least one handler for this event ({name}).")
    {
    }
}