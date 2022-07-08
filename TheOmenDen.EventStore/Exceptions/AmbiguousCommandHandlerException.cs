namespace TheOmenDen.EventStore.Exceptions;
internal class AmbiguousCommandHandlerException : Exception
{
    public AmbiguousCommandHandlerException(String name)
    :base($"You cannot define multiple handlers for the same command ({name}).")
    {
    }
}
