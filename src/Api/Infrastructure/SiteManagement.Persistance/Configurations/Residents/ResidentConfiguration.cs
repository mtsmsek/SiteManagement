using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SiteManagement.Domain.Entities.Residents;
using SiteManagement.Persistance.Configurations.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Persistance.Configurations.Residents
{
    public class ResidentConfiguration : BaseEntityConfiguration<Resident>
    {
        public override void Configure(EntityTypeBuilder<Resident> builder)
        {
            builder.Property(resident => resident.ApartmentId).IsRequired();
            builder.Property(resident => resident.FirstName).IsRequired();
            builder.Property(resident => resident.LastName).IsRequired();
            builder.Property(resident => resident.IdenticalNumber).IsRequired();
            builder.Property(resident => resident.PhoneNumber).IsRequired();
            builder.Property(resident => resident.Email).IsRequired();

            builder.HasOne(resident => resident.Apartment)
                   .WithMany(apartment => apartment.Residents)
                   .HasForeignKey(resident => resident.ApartmentId);

        }
    }
}
