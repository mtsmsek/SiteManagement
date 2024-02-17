using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SiteManagement.Domain.Entities.Buildings;
using SiteManagement.Domain.Enumarations.Buildings;
using SiteManagement.Persistance.Configurations.Commons;

namespace SiteManagement.Persistance.Configurations.Buildings;

public class ApartmentConfiguration : BaseEntityConfiguration<Apartment>
{
    public override void Configure(EntityTypeBuilder<Apartment> builder)
    {
        builder.Property(apartment => apartment.BlockId).IsRequired();
        builder.Property(apartment => apartment.Status).IsRequired();
        builder.Property(apartment => apartment.ApartmentType).HasConversion(
            apartmentType => apartmentType.Value,
            value => ApartmentType.FromValue(value)!);
        builder.Property(apartment => apartment.ApartmentNumber).IsRequired();
        builder.Property(apartment => apartment.FloorNumber).IsRequired();
        builder.Property(apartment => apartment.IsTenant).IsRequired();


        builder.HasOne(apartment => apartment.Block)
               .WithMany(block => block.Apartments)
               .HasForeignKey(apartment => apartment.BlockId);
    }

   
}
