using MediatR;
using SiteManagement.Application.Pagination.Responses;

namespace SiteManagement.Application.Features.Queries.Residents.GetResidentsByFullName;

public class GetResidentsByFullNameQuery : IRequest<PagedViewModel<GetResidentsByFullNameResponse>>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}
