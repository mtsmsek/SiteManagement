using MediatR;
using SiteManagement.Application.Pagination.Responses;

namespace SiteManagement.Application.Features.Queries.Residents.GetListResidentsByVehicle
{
    public class GetListResidentsByVehicleQuery : IRequest<PagedViewModel<GetListResidentsByVehicleResponse>>
    {
        public string VehicleRegistrationPlate { get; set; }
    }
}
