using MediatR;
using SiteManagement.Application.Pagination.Responses;

namespace SiteManagement.Application.Features.Queries.Residents.GetListResidentByApartmentNumberAndBlockName;

public class GetListResidentByApartmentNumberAndBlockNameQuery : IRequest<PagedViewModel<GetListResidentByApartmentNumberAndBlockNameResponse>>
{
    public string BlockName { get; set; }
    public int ApartmentNumber { get; set; }
}
