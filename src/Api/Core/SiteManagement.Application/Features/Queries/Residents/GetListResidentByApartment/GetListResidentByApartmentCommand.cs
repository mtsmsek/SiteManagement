using MediatR;
using SiteManagement.Application.Pagination.Responses;

namespace SiteManagement.Application.Features.Queries.Residents.GetListResidentByApartment;

public class GetListResidentByApartmentCommand : IRequest<PagedViewModel<GetListResidentByApartmentResponse>>
{
    public string BlockName { get; set; }
    public int ApartmentNumber { get; set; }
}
