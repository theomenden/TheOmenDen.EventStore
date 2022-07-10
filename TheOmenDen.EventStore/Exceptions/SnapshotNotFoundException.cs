namespace TheOmenDen.EventStore.Exceptions;

internal class SnapshotNotFoundException : Exception
{
    public SnapshotNotFoundException(String snapshotFileName)
        :base($"Couldn't locate a snapshot at the specified location {snapshotFileName}")
    {
    }
}
