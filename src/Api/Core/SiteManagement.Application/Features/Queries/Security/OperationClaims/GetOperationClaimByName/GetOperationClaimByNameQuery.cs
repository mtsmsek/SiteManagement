using MediatR;

namespace SiteManagement.Application.Features.Queries.Security.OperationClaims.GetOperationClaimByName;

public class GetOperationClaimByNameQuery : IRequest<GetOperationClaimByNameResponse>
{
    public string Name { get; set; }
}
