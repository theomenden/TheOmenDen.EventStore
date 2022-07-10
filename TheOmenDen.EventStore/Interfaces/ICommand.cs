namespace TheOmenDen.EventStore.Interfaces;
public interface ICommand
{
    Guid Id { get; set; }

    Int32? ExpectedVersion { get; set; }

    Guid IdentityTenant { get; set; }

    Guid IdentityUser { get; set; }

    Guid AggregateIdentifier { get; set; } 
}

