namespace TheOmenDen.EventStore.Interfaces;
public interface ICommandQueue
{
    #region Subscription Methods
    void Subscribe<T>(Action<T> action) where T : ICommand;

    Task SubscribeAsync<T>(Action<T> action, CancellationToken cancellationToken = new()) where T : ICommand;
    #endregion
    #region Override Methods
    void Override<T>(Action<T> action, Guid tenant) where T : ICommand;

    Task OverrideAsync<T>(Action<T> action, Guid tenant, CancellationToken cancellationToken = new()) where T : ICommand;
    #endregion
    #region Schedule Methods
    void Send(ICommand command);

    void Schedule(ICommand command, DateTimeOffset scheduledAt);

    void Ping();

    void Start(Guid command);

    void Cancel(Guid command);

    void Complete(Guid command);
    #endregion
    #region Async Schedule Methods
    Task SendAsync(ICommand command, CancellationToken cancellationToken = new());

    Task ScheduleAsync(ICommand command, DateTimeOffset scheduledAt, CancellationToken cancellationToken = new());

    Task PingAsync(CancellationToken cancellationToken = new());

    Task StartAsync(Guid command, CancellationToken cancellationToken = new());

    Task CancelAsync(Guid command, CancellationToken cancellationToken = new());

    Task CompleteAsync(Guid command, CancellationToken cancellationToken = new());
    #endregion
}