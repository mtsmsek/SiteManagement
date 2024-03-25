using MediatR;
using SiteManagement.Application.Pagination.Responses;

namespace SiteManagement.Application.Features.Queries.ResidentVehicles.GetListResidentVehicles;

public class GetListResidentVehiclesQuery : IRequest<PagedViewModel<GetListResidentVehiclesResponse>>
{
    public Guid ResidentId { get; set; }
}
