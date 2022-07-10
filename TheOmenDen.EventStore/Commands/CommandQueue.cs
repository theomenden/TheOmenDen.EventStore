using TheOmenDen.EventStore.Identities;

namespace TheOmenDen.EventStore.Commands;
public class CommandQueue: ICommandQueue
{
    private readonly Dictionary<String, Action<ICommand>> _subscribers;

    private readonly Dictionary<CommandOverrideKey, Action<ICommand>> _overriders;

    private readonly ICommandStore _store;

    private readonly IIdentityService _service;

    private readonly Boolean _saveAll;

    public CommandQueue(ICommandStore store, IIdentityService service, bool saveAll = false)
    {
        _subscribers = new Dictionary<String, Action<ICommand>>();
        _overriders = new Dictionary<CommandOverrideKey, Action<ICommand>>();
        _store = store;
        _service = service;
        _saveAll = saveAll;
    }
    #region Subscription
    public void Subscribe<T>(Action<T> action) where T : ICommand
    {
        var name = typeof(T).GetClassName();

        if (_subscribers.Any(x => x.Key == name))
        {
            throw new AmbiguousCommandHandlerException(name);
        }

        _subscribers.Add(name, (command) => action((T)command));
    }

    public Task SubscribeAsync<T>(Action<T> action, CancellationToken cancellationToken = new CancellationToken()) where T : ICommand
    {
        throw new NotImplementedException();
    }

    public void Override<T>(Action<T> action, Guid tenant) where T : ICommand
    {
        var key = new CommandOverrideKey
        {
            Name = typeof(T).GetClassName(),
            IdentityTenant = tenant
        };

        if (_overriders.Any(x => x.Key.Name == key.Name && x.Key.IdentityTenant == key.IdentityTenant))
        {
            throw new AmbiguousCommandHandlerException(key.Name);
        }

        _overriders.Add(key, (command) => action((T)command));
    }

    public Task OverrideAsync<T>(Action<T> action, Guid tenant, CancellationToken cancellationToken = new CancellationToken()) where T : ICommand
    {
        throw new NotImplementedException();
    }
    #endregion
    #region Synchronous Send Methods
    public void Send(ICommand command)
    {
        Identify(command);

        var savedCommand = default(SavedCommand);

        if (_saveAll)
        {
            savedCommand = new SavedCommand(command)
            {
                SendStartedAt = DateTimeOffset.UtcNow
            };
        }

        Execute(command);

        if (!_saveAll || savedCommand is null)
        {
            return;
        }

        savedCommand.SendCompletedAt = DateTimeOffset.UtcNow;
        savedCommand.SendStatus = "Completed";
        _store.Save(savedCommand, true);
    }

    public void Schedule(ICommand command, DateTimeOffset scheduledAt)
    {
        Identify(command);

        var serialized = new SavedCommand(command)
        {
            SendScheduledAt = scheduledAt,
            SendStatus = "Scheduled"
        };

        _store.Save(serialized, true);
    }

    public void Ping()
    {
        var commands = _store.GetExpired(DateTimeOffset.UtcNow);

        foreach (var command in commands)
        {
            Execute(command);
        }
    }

    public void Start(Guid command)
    {
        Execute(_store.Get(command));
    }

    public void Cancel(Guid command)
    {
        var serialized = _store.Get(command);

        serialized.SendCancelledAt = DateTimeOffset.UtcNow;
        serialized.SendStatus = "Cancelled";

        _store.Save(serialized, false);
    }

    public void Complete(Guid command)
    {
        var serialized = _store.Get(command);

        serialized.SendCompletedAt = DateTimeOffset.UtcNow;
        serialized.SendStatus = "Completed";

        _store.Save(serialized, false);
    }
    #endregion
    #region Asynchronous Send Methods
    public Task SendAsync(ICommand command, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public Task ScheduleAsync(ICommand command, DateTimeOffset scheduledAt,
        CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public Task PingAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public Task StartAsync(Guid command, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public Task CancelAsync(Guid command, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public Task CompleteAsync(Guid command, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }
    #endregion
    #region Private Methods
    private void Execute(ICommand command)
    {
        var name = command.GetType().GetClassName();

        if (_overriders.Keys.Any(k => k.Name == name && k.IdentityTenant == command.IdentityTenant))
        {
            var customization = _overriders
                .First(kv => kv.Key.Name == name && kv.Key.IdentityTenant == command.IdentityTenant)
                .Value;

            customization?.Invoke(command);

            return;
        }

        if (!_subscribers.ContainsKey(name))
        {
            throw new UnhandledCommandException(name);
        }

        var action = _subscribers[name];
        action?.Invoke(command);
    }


    private void Execute(SavedCommand serialized)
    {
        serialized.SendStartedAt = DateTimeOffset.UtcNow;
        serialized.SendStatus = "Started";

        _store.Save(serialized, false);

        Execute(serialized.Command);

        serialized.SendCompletedAt = DateTimeOffset.UtcNow;
        serialized.SendStatus = "Completed";
        _store.Save(serialized, false);
    }

    private void Identify(ICommand command)
    {
        var current = _service.GetCurrent();
        var tenant = current.Tentant.Id;
        var user = current.User.Id;

        command.IdentityTenant = Guid.Empty == command.IdentityTenant ? tenant : command.Id;
        command.IdentityUser = Guid.Empty == command.IdentityUser ? user : command.IdentityUser;
    }
    #endregion
}