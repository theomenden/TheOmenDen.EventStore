using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TheOmenDen.EventStore.Persistence.Configurations;

internal partial class SerializedEventConfiguration : IEntityTypeConfiguration<SerializedEvent>
{
    public void Configure(EntityTypeBuilder<SerializedEvent> entity)
    {
        entity.ToTable($"{nameof(SerializedEvent)}s", "Logging");

        entity.HasKey(e => e.Id)
            .IsClustered()
            .HasName("PK_SerializedEvent_Id");

        entity.Property(e => e.Id)
            .HasDefaultValueSql("(newsequentialid())");

        entity.Property(e => e.UnderlyingType)
            .HasConversion(e => e.FullName
                , t => Type.GetType(t) ?? Type.EmptyTypes[0]);

        entity.HasIndex(e => new { e.MajorVersion, e.MinorVersion })
            .IsClustered(false)
            .HasDatabaseName("IX_EventMajorMinorVersion")
            .IncludeProperties(e => new
            {
                e.EventType,
                e.EventData
            });


        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<SerializedEvent> builder);
}
