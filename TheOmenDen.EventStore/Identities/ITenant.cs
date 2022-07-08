namespace TheOmenDen.EventStore.Identities;
public interface ITenant
{
    Guid Id { get; }

    String Code { get; }

    String Name { get; }

    Int32 Key { get; }
}

