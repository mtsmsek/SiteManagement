using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SiteManagement.Domain.Entities.Vehicles;
using SiteManagement.Persistance.Configurations.Commons;

namespace SiteManagement.Persistance.Configurations.Vehicles;

public class ResidentVehicleConfiguration : BaseEntityConfiguration<ResidentVehicle>
{
    public override void Configure(EntityTypeBuilder<ResidentVehicle> builder)
    {
        
        builder.Property(residentVehicle => residentVehicle.ResidentId).IsRequired();
        builder.Property(residentVehicle => residentVehicle.VehicleId).IsRequired();

        builder.HasOne(residentVehicle => residentVehicle.Vehicle)
                .WithMany(resident => resident.Residents)
                .HasForeignKey(residentVehicle => residentVehicle.VehicleId);

        builder.HasOne(residentVehicle => residentVehicle.Resident)
                .WithMany(resident => resident.Vehicles)
                .HasForeignKey(residentVehicle => residentVehicle.ResidentId);

        builder.HasIndex(residentVehicle => new
        {
            residentVehicle.ResidentId,
            residentVehicle.VehicleId
        });

    }
}
