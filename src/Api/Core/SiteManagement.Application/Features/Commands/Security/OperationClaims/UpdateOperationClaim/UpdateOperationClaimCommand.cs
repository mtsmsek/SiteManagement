using MediatR;

namespace SiteManagement.Application.Features.Commands.Security.OperationClaims.UpdateOperationClaim;

public class UpdateOperationClaimCommand : IRequest<UpdateOperationClaimResponse>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}
