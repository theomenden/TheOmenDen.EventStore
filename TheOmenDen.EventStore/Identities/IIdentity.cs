namespace TheOmenDen.EventStore.Identities;

public interface IIdentity
{
    ITenant Tentant { get; }

    IUser User { get; }
}
