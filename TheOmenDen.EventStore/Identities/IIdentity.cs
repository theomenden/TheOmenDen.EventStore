namespace TheOmenDen.EventStore.Identities;

public interface IIdentity
{
    Tenant Tentant { get; }

    User User { get; }
}
