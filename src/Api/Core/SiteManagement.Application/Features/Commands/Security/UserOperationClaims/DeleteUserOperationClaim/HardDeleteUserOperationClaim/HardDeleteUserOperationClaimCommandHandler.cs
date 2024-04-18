using MediatR;
using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;
using SiteManagement.Application.Services.Repositories.Security;

namespace SiteManagement.Application.Features.Commands.Security.UserOperationClaims.DeleteUserOperationClaim.HardDeleteUserOperationClaim;

public class HardDeleteUserOperationClaimCommandHandler : IRequestHandler<HardDeleteUserOperationClaimCommand>
{
    private readonly IUserOperationClaimRepository _userOperationClaimRepository;

    public HardDeleteUserOperationClaimCommandHandler(IUserOperationClaimRepository userOperationClaimRepository)
    {
        _userOperationClaimRepository = userOperationClaimRepository;
    }

    public async Task Handle(HardDeleteUserOperationClaimCommand request, CancellationToken cancellationToken)
    {
        var userOperationClaimToDelete = await _userOperationClaimRepository.GetByIdAsync(request.Id);
        if (userOperationClaimToDelete == null)
            throw new BusinessException("User operation claim cannot be found");

        await _userOperationClaimRepository.DeleteAsync(entity: userOperationClaimToDelete, isPermenant: true, cancellationToken: cancellationToken);
        
    }
}

