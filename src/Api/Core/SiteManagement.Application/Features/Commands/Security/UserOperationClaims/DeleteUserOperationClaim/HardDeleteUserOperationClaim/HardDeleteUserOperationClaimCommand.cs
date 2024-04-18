using MediatR;

namespace SiteManagement.Application.Features.Commands.Security.UserOperationClaims.DeleteUserOperationClaim.HardDeleteUserOperationClaim;

public class HardDeleteUserOperationClaimCommand : IRequest
{
    public  Guid Id { get; set; }

}
