using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SiteManagement.Domain.Entities.Vehicles;
using SiteManagement.Persistance.Configurations.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Persistance.Configurations.Vehicles
{
    public class VehicleConfiguration : BaseEntityConfiguration<Vehicle>
    {
        public override void Configure(EntityTypeBuilder<Vehicle> builder)
        {
            builder.Property(vehicle => vehicle.VehicleRegistrationPlate).IsRequired();
            builder.OwnsOne(vehicle => vehicle.VehicleType);

        }
    }
}
