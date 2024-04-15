using MediatR;

namespace SiteManagement.Application.Features.Commands.Security.OperationClaims.CreateOperationClaim;

public class CreateOperationClaimCommand : IRequest<CreateOperationClaimResponse>
{
    public string Name { get; set; }
}
