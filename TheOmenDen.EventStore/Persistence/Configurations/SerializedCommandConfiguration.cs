using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TheOmenDen.EventStore.Persistence.Configurations
{
    internal partial class SerializedCommandConfiguration: IEntityTypeConfiguration<SerializedCommand>
    {
        public void Configure(EntityTypeBuilder<SerializedCommand> entity)
        {
            entity.ToTable($"{nameof(SerializedCommand)}s", "Logging");

            entity.HasKey(e => e.Id)
                .IsClustered()
                .HasName("PK_SerializedCommand_Id");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newsequentialid())");

            entity.Property(e => e.AggregateIdentifier)
                .HasColumnName("AggregateId");

            entity.HasIndex(e => e.ExpectedVersion)
                .IsClustered(false)
                .HasDatabaseName("IX_CommandExpectedVersion")
                .IncludeProperties(e => new
                {
                    e.SendStatus,
                    e.CommandData
                });

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<SerializedCommand> builder);
    }
}
