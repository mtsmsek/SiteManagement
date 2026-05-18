using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SiteManagement.Domain.Property;

namespace SiteManagement.Infrastructure.Persistence.Configurations.Property;

/// <summary>
/// EF mapping for the <see cref="Site"/> aggregate root. Stores blocks as a
/// dependent collection (1:N with cascade delete); domain events are
/// ignored — they live in memory and are dispatched by the change tracker
/// scan during <c>SaveChangesAsync</c>.
/// </summary>
public sealed class SiteConfiguration : IEntityTypeConfiguration<Site>
{
    public void Configure(EntityTypeBuilder<Site> builder)
    {
        builder.ToTable(SchemaConstants.Tables.Sites);

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(PropertyLimits.SiteNameMaxLength);

        builder.Property(s => s.Address)
            .IsRequired()
            .HasMaxLength(PropertyLimits.SiteAddressMaxLength);

        builder.HasMany(s => s.Blocks)
            .WithOne()
            .HasForeignKey(SchemaConstants.ForeignKeys.SiteId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(s => s.Blocks)
            .HasField(SchemaConstants.BackingFields.SiteBlocks)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Ignore(s => s.DomainEvents);
    }
}
