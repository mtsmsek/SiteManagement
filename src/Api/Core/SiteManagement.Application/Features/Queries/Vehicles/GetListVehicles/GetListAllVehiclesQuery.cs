using MediatR;
using SiteManagement.Application.Pagination.Responses;

namespace SiteManagement.Application.Features.Queries.Vehicles.GetListVehicles;

public class GetListAllVehiclesQuery : IRequest<PagedViewModel<GetListAllVehiclesResponse>>
{
}
