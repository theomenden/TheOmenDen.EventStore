namespace TheOmenDen.EventStore.Identities;

public interface IIdentityService
{
    IIdentity GetCurrent();

    ValueTask<IIdentity> GetCurrentAsync(CancellationToken cancellationToken = new());
}

