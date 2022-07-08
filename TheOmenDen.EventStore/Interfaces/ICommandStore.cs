using TheOmenDen.EventStore.Commands;

namespace TheOmenDen.EventStore.Interfaces;
public interface ICommandStore
{
    bool Exists(Guid commandId);

    SavedCommand Get(Guid command);

    IEnumerable<SavedCommand> GetExpired(DateTimeOffset expiredAt);

    void Save(SavedCommand command, bool isNew);
}