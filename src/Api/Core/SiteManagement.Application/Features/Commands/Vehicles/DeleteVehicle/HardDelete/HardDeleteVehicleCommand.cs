using MediatR;

namespace SiteManagement.Application.Features.Commands.Vehicles.DeleteCehicle.HardDelete
{
    public class HardDeleteVehicleCommand : IRequest<int>
    {
        public Guid VehicleId { get; set; }
        public Guid UserId { get; set; }

    }
}
