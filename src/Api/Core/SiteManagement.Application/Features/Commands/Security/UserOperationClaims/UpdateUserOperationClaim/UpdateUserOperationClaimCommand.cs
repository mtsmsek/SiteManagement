using MediatR;

namespace SiteManagement.Application.Features.Commands.Security.UserOperationClaims.UpdateUserOperationClaim;

public class UpdateUserOperationClaimCommand: IRequest<UpdateUserOperationClaimResponse>
{
    public Guid UserId { get; set; }
    public Guid OperationClaimId { get; set; }
}
