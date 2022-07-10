using MediatR;

namespace TheOmenDen.EventStore.Adapters;
public class MediatrToEventStoreAdapter
{
    private readonly ICommandQueue _commandQueue;
    private readonly IMediator _mediator;

    public MediatrToEventStoreAdapter(ICommandQueue commandQueue, IMediator mediator)
    {
        _commandQueue = commandQueue;
        _mediator = mediator;
    }

    public Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default)
    where TRequest : ICommand, IRequest
    {
        var isInTransaction = StartTransaction(nameof(TRequest));
        try
        {
            _commandQueue.Schedule(request, DateTimeOffset.UtcNow);

            if (cancellationToken.IsCancellationRequested)
            {
                _commandQueue.Cancel(request.Id);
            }

            _commandQueue.Start(request.Id);
        }
        finally
        {
            if (isInTransaction)
            {
                EndTransaction(nameof(TRequest));
            }
        }

        return _mediator.Send(request, cancellationToken);
    }

    private static bool StartTransaction(String key)
    {
        try
        {
            var transactionLock = LockInstance<IRequest>.Create(key);

            transactionLock.Wait();
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static void EndTransaction(String key)
    {
        var transactionLock = LockInstance<IRequest>.Get(key);

        transactionLock.Release();
    }
}

