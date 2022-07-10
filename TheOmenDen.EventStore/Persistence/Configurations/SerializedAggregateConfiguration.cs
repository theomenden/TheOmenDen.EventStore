using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TheOmenDen.EventStore.Persistence.Configurations
{
    internal partial class SerializedAggregateConfiguration : IEntityTypeConfiguration<SerializedAggregate>
    {
        public void Configure(EntityTypeBuilder<SerializedAggregate> entity)
        {
            entity.ToTable("Aggregates", "Logging");

            entity.HasKey(e => e.Id)
                .IsClustered()
                .HasName("PK_Aggregate_Id");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newsequentialid())");

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<SerializedAggregate> entity);
    }
}
