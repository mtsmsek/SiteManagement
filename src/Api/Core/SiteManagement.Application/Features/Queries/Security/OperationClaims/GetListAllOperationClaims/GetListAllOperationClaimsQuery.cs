using MediatR;
using SiteManagement.Application.Pagination.Requests;
using SiteManagement.Application.Pagination.Responses;

namespace SiteManagement.Application.Features.Queries.Security.OperationClaims.GetListAllOperationClaims;

public class GetListAllOperationClaimsQuery: PageRequest ,IRequest<PagedViewModel<GetListAllOperationClaimsResponse>>
{

}
