using MediatR;

namespace SiteManagement.Application.Features.Commands.Security.UserOperationClaims.CreateUserOperationClaim;

public class CreateUserOperationClaimCommand : IRequest<CreateUserOperationClaimResponse>
{
    public Guid UserId { get; set; }
    public Guid OperationClaimId { get; set; }

}
