using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SiteManagement.Infrastructure.Persistence.Outbox;

namespace SiteManagement.Infrastructure.Persistence.Configurations.Outbox;

/// <summary>
/// EF mapping for <see cref="OutboxMessage"/>. A filtered index on the pending
/// rows keeps the processor's "give me unprocessed, oldest first" poll cheap as
/// the delivered backlog grows.
/// </summary>
public sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable(SchemaConstants.Tables.OutboxMessages);

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Type).IsRequired();
        builder.Property(m => m.Content).IsRequired();
        builder.Property(m => m.OccurredOnUtc).IsRequired();
        builder.Property(m => m.ProcessedOnUtc);
        builder.Property(m => m.Error);

        builder.HasIndex(m => m.OccurredOnUtc)
            .HasFilter($"\"{nameof(OutboxMessage.ProcessedOnUtc)}\" IS NULL");
    }
}
