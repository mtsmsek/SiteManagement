using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SiteManagement.Domain.Property;
using SiteManagement.Infrastructure.Persistence.Converters;

namespace SiteManagement.Infrastructure.Persistence.Configurations.Property;

/// <summary>
/// EF mapping for the inner <see cref="Apartment"/> entity. Lives inside
/// the <c>Site</c> aggregate (via Block); the shadow <c>BlockId</c> foreign
/// key is created by <see cref="BlockConfiguration"/>.
/// </summary>
public sealed class ApartmentConfiguration : IEntityTypeConfiguration<Apartment>
{
    public void Configure(EntityTypeBuilder<Apartment> builder)
    {
        builder.ToTable(SchemaConstants.Tables.Apartments);

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Number)
            .HasConversion(new ApartmentNumberConverter());

        builder.Property(a => a.Floor)
            .HasConversion(new FloorConverter());

        builder.Property(a => a.Type)
            .HasConversion(new ApartmentTypeConverter())
            .HasMaxLength(PropertyLimits.ApartmentTypeMaxLength);

        builder.Property(a => a.Status)
            .HasConversion<string>()
            .HasMaxLength(PropertyLimits.OccupancyStatusMaxLength);
    }
}
