using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SiteManagement.Domain.Entities.Vehicles;
using SiteManagement.Domain.Enumarations.Vehicles;
using SiteManagement.Persistance.Configurations.Commons;

namespace SiteManagement.Persistance.Configurations.Vehicles;

public class VehicleConfiguration : BaseEntityConfiguration<Vehicle>
{
    public override void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.Property(vehicle => vehicle.VehicleRegistrationPlate).IsRequired();
        builder.Property(vehicle => vehicle.VehicleType).HasConversion(
                         vehicleType => vehicleType.Value,
                         value => VehicleType.FromValue(value)!);

    }
}
