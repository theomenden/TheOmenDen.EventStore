namespace TheOmenDen.EventStore.Extensions;
#nullable disable
public static class CommandExtensions
{
    public static SavedCommand Deserialize(this SerializedCommand serializedCommand)
    {
        var data = JsonSerializer.Deserialize<ICommand>(serializedCommand.CommandData);

        data.AggregateIdentifier = serializedCommand.AggregateIdentifier;
        data.ExpectedVersion = serializedCommand.ExpectedVersion;
        data.Id = serializedCommand.Id;
        data.IdentityTenant = serializedCommand.IdentityTenant;
        data.IdentityUser = serializedCommand.IdentityUser;

        var savedCommand = new SavedCommand(data)
        {
            SendStatus = serializedCommand.SendStatus,
            SendError = serializedCommand.SendError,
            SendScheduledAt = serializedCommand.SendScheduled,
            SendStartedAt = serializedCommand.SendStarted,
            SendCompletedAt = serializedCommand.SendCompleted,
            SendCancelledAt = serializedCommand.SendCancelled
        };

        return savedCommand;
    }

    public static SerializedCommand Serialize(this SavedCommand commandSchedule)
    {
        var data = JsonSerializer.Serialize(commandSchedule.Command);

        var serialized = new SerializedCommand
        {
            AggregateIdentifier = commandSchedule.Command.AggregateIdentifier,
            ExpectedVersion = commandSchedule.Command.ExpectedVersion,

            CommandClass = commandSchedule.Command.GetType().GetClassName(),
            CommandType = commandSchedule.Command.GetType().Name,
            CommandData = data,

            CommandIdentifier = commandSchedule.Command.Id,

            IdentityTenant = commandSchedule.Command.IdentityTenant,
            IdentityUser = commandSchedule.Command.IdentityUser,

            SendStatus = commandSchedule.SendStatus,
            SendError = commandSchedule.SendError,

            SendScheduled = commandSchedule.SendScheduledAt,
            SendStarted = commandSchedule.SendStartedAt,
            SendCompleted = commandSchedule.SendCompletedAt,
            SendCancelled = commandSchedule.SendCancelledAt,
        };

        if (serialized.CommandClass.Length > 200)
        {
            throw new OverflowException($"The assembly-qualified name for this command ({serialized.CommandClass}) exceeds the maximum character limit (200).");
        }

        if (serialized.CommandType.Length > 100)
        {
            throw new OverflowException($"The type name for this command ({serialized.CommandType}) exceeds the maximum character limit (100).");
        }

        if ((serialized.SendStatus?.Length ?? 0) > 20)
        {
            throw new OverflowException($"The send status ({serialized.SendStatus}) exceeds the maximum character limit (20).");
        }

        return serialized;
    }
}