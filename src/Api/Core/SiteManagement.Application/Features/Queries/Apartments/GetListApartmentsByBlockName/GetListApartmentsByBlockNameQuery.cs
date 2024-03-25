using MediatR;
using SiteManagement.Application.Pagination.Responses;

namespace SiteManagement.Application.Features.Queries.Apartments.GetListApartmentsByBlockName;

public class GetListApartmentsByBlockNameQuery : IRequest<PagedViewModel<GetListApartmentsByBlockNameResponse>>
{
    public string BlockName { get; set; }
}
