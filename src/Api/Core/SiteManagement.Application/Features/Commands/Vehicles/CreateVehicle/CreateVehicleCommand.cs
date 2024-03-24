using MediatR;
using SiteManagement.Domain.Entities.Buildings;
using SiteManagement.Domain.Enumarations.Vehicles;

namespace SiteManagement.Application.Features.Commands.Vehicles.CreateVehicle;

public class CreateVehicleCommand : IRequest<CreateVehicleResponse>
{
    //To add vehicle to all partner of apartment
    public Guid ApartmentId { get; set; }
    public string VehicleRegistrationPlate { get; set; }
    public VehicleType VehicleType { get; set; }
}
