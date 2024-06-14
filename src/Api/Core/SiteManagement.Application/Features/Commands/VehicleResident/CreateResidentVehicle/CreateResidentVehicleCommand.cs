using MediatR;

namespace SiteManagement.Application.Features.Commands.VehicleResident.CreateVehicleResident;

public class CreateResidentVehicleCommand : IRequest<int>
{
    public Guid VehicleId { get; set; }
    public Guid ResidentId { get; set; }

}
