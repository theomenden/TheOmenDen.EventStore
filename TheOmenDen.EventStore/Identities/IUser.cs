namespace TheOmenDen.EventStore.Identities; 
public interface IUser
{
    Guid Id { get; }

    String Email { get; }

    String Name { get; }

    Boolean IsAuthenticated { get; }

    Int32 Key { get; }
}