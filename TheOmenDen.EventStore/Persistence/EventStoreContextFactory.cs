using Microsoft.EntityFrameworkCore.Design;

namespace TheOmenDen.EventStore.Persistence;

public class EventStoreContextFactory: IDesignTimeDbContextFactory<EventStoreContext>
{
    private const string CONNECTION_STRING = "Server=(local);Database=EventStore.db;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true";


    public EventStoreContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<EventStoreContext>();
        optionsBuilder.UseSqlServer(CONNECTION_STRING)
            .EnableServiceProviderCaching();

        return new EventStoreContext(optionsBuilder.Options);
    }
}
