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

        // Use Postgres' built-in `xmin` system column as the optimistic
        // concurrency token (shadow uint property; npgsql convention
        // detects "uint OnAddOrUpdate concurrency token" and maps it to
        // xmin). Stops EF from emitting empty UPDATEs when an aggregate's
        // child collection changes but no scalar property does (e.g.
        // site.AddBlock without touching Name/Address), and gives us real
        // concurrency safety for free.
        builder.Property<uint>(SchemaConstants.ConcurrencyTokenColumn)
            .HasColumnName(SchemaConstants.ConcurrencyTokenColumn)
            .IsConcurrencyToken()
            .ValueGeneratedOnAddOrUpdate();

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

        // Soft delete: archived sites (Site.Archive) are hidden from every read.
        // A hard purge bypasses this with IgnoreQueryFilters + a real delete.
        builder.HasQueryFilter(s => !s.IsDeleted);

        builder.Ignore(s => s.DomainEvents);
    }
}
