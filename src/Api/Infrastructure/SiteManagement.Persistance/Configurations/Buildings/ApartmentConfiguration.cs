using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SiteManagement.Domain.Entities.Buildings;
using SiteManagement.Persistance.Configurations.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Persistance.Configurations.Buildings
{
    public class ApartmentConfiguration : BaseEntityConfiguration<Apartment>
    {
        public override void Configure(EntityTypeBuilder<Apartment> builder)
        {
            builder.Property(apartment => apartment.BlockId).IsRequired();
            builder.Property(apartment => apartment.Status).IsRequired();
            builder.OwnsOne(apartment => apartment.ApartmentType);
            builder.Property(apartment => apartment.ApartmentNumber).IsRequired();
            builder.Property(apartment => apartment.FloorNumber).IsRequired();
            builder.Property(apartment => apartment.IsTenant).IsRequired();


            builder.HasOne(apartment => apartment.Block)
                   .WithMany(block => block.Apartments)
                   .HasForeignKey(apartment => apartment.BlockId);
        }
    }
}
