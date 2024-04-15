using MediatR;

namespace SiteManagement.Application.Features.Commands.Security.OperationClaims.DeleteOperationClaim.HardDelete;

public class HardDeleteOperationClaimCommand : IRequest<Guid>
{
    public Guid Id { get; set; }
}
