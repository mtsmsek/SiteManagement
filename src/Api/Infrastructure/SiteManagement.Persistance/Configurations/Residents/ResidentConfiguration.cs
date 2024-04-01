using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SiteManagement.Domain.Entities.Residents;
using SiteManagement.Persistance.Configurations.Commons;

namespace SiteManagement.Persistance.Configurations.Residents;

public class ResidentConfiguration : BaseEntityConfiguration<Resident>
{
    public override void Configure(EntityTypeBuilder<Resident> builder)
    {

        builder.ToTable("Residents");
        builder.Property(resident => resident.ApartmentId).IsRequired();
        

        builder.Property(resident => resident.IdenticalNumber).IsRequired();
        builder.Property(resident => resident.PhoneNumber).IsRequired();



        builder.HasOne(resident => resident.Apartment)
               .WithMany(apartment => apartment.Residents)
               .HasForeignKey(resident => resident.ApartmentId);

    }
}
