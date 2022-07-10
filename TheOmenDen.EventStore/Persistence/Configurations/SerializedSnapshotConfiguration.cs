using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TheOmenDen.EventStore.Persistence.Configurations;
internal partial class SerializedSnapshotConfiguration : IEntityTypeConfiguration<SerializedSnapshot>
{
    public void Configure(EntityTypeBuilder<SerializedSnapshot> entity)
    {
        entity.ToTable("Snapshots", "Logging");

        entity.HasKey(e => e.Id)
            .IsClustered()
            .HasName("PK_Snapshot_Id");

        entity.Property(e => e.Id)
            .HasDefaultValueSql("(newsequentialid())");
        
        entity.HasIndex(e => new { e.AggregateMajorVersion, e.AggregateMinorVersion })
            .HasDatabaseName("IX_Snapshot_AggregateMajorMinorVersion");

        entity.HasIndex(e => e.AggregateId)
            .HasDatabaseName("IX_Snapshot_AggregateId");

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<SerializedSnapshot> entity);
}

