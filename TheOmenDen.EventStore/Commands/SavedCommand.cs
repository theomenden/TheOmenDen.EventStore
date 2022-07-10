namespace TheOmenDen.EventStore.Commands;
#nullable disable
public class SavedCommand
{
    public SavedCommand(ICommand command)
    {
        Command = command;
    }

    public ICommand Command { get; set; }

    public DateTimeOffset? SendScheduledAt { get; set; }
    public DateTimeOffset? SendStartedAt { get; set; }
    public DateTimeOffset? SendCompletedAt { get; set; }
    public DateTimeOffset? SendCancelledAt { get; set; }

    public String SendStatus { get; set; }

    public String SendError { get; set; }
}

