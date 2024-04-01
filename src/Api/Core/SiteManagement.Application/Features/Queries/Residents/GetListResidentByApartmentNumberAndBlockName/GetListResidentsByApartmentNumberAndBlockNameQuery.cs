using MediatR;
using SiteManagement.Application.Pagination.Responses;

namespace SiteManagement.Application.Features.Queries.Residents.GetListResidentByApartmentNumberAndBlockName;

public class GetListResidentsByApartmentNumberAndBlockNameQuery : IRequest<PagedViewModel<GetListResidentsByApartmentNumberAndBlockNameResponse>>
{
    public string BlockName { get; set; }
    public int ApartmentNumber { get; set; }
}
