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
    public class ResidentVehcleConfiguration : BaseEntityConfiguration<ResidentVehicle>
    {
        public override void Configure(EntityTypeBuilder<ResidentVehicle> builder)
        {
            builder.Property(residentVehicle => residentVehicle.ResidentId).IsRequired();
            builder.Property(residentVehicle => residentVehicle.VehicleId).IsRequired();

            builder.HasOne(residentVehicle => residentVehicle.Resident);
            builder.HasOne(residentVehicle => residentVehicle.Vehicle) ;
                   
        }
    }
}
