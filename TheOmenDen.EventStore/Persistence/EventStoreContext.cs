using Microsoft.EntityFrameworkCore;
using TheOmenDen.EventStore.Persistence.Configurations;

namespace TheOmenDen.EventStore.Persistence;

public partial class EventStoreContext: DbContext
{
    public virtual DbSet<SerializedSnapshot> SerializedSnapshots { get; set; } = null!;

    public virtual DbSet<SerializedAggregate> SerializedAggregates { get; set; } = null!;

    public virtual DbSet<SerializedEvent> SerializedEvents { get; set; } = null!;

    public virtual DbSet<SerializedCommand> SerializedCommands { get; set; } = null!;
    
    public EventStoreContext(DbContextOptions<EventStoreContext> options)
    : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new SerializedAggregateConfiguration());

        modelBuilder.ApplyConfiguration(new SerializedEventConfiguration());

        modelBuilder.ApplyConfiguration(new SerializedCommandConfiguration());

        modelBuilder.ApplyConfiguration(new SerializedSnapshotConfiguration());

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
