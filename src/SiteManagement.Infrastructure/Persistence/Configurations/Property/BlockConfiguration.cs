using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SiteManagement.Domain.Property;
using SiteManagement.Infrastructure.Persistence.Converters;

namespace SiteManagement.Infrastructure.Persistence.Configurations.Property;

/// <summary>
/// EF mapping for the inner <see cref="Block"/> entity. Lives inside the
/// <c>Site</c> aggregate; the shadow <c>SiteId</c> foreign key is created
/// by <see cref="SiteConfiguration"/>.
/// </summary>
public sealed class BlockConfiguration : IEntityTypeConfiguration<Block>
{
    public void Configure(EntityTypeBuilder<Block> builder)
    {
        builder.ToTable(SchemaConstants.Tables.Blocks);

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Name)
            .HasConversion(new BlockNameConverter())
            .IsRequired()
            .HasMaxLength(PropertyLimits.BlockNameMaxLength);

        builder.HasMany(b => b.Apartments)
            .WithOne()
            .HasForeignKey(SchemaConstants.ForeignKeys.BlockId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(b => b.Apartments)
            .HasField(SchemaConstants.BackingFields.BlockApartments)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
