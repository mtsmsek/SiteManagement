using MediatR;
using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;
using SiteManagement.Application.Services.Repositories.Security;

namespace SiteManagement.Application.Features.Commands.Security.OperationClaims.DeleteOperationClaim.HardDelete;

public class HardDeleteOperationClaimCommandHandler : IRequestHandler<HardDeleteOperationClaimCommand, Guid>
{
    private readonly IOperationClaimRepository _operationClaimRepository;

    public HardDeleteOperationClaimCommandHandler(IOperationClaimRepository operationClaimRepository)
    {
        _operationClaimRepository = operationClaimRepository;
    }

    public async Task<Guid> Handle(HardDeleteOperationClaimCommand request, CancellationToken cancellationToken)
    {

        //todo remove magic string
        if (request.Id == Guid.Empty)
            throw new BusinessException("Id boş olamaz");

        var operationClaim = await _operationClaimRepository.GetByIdAsync(request.Id);
        if (operationClaim is null)
            throw new BusinessException("Aradığınız operation cliam bulunamadı");

        await _operationClaimRepository.DeleteAsync(operationClaim, true, cancellationToken);
        return request.Id;
    }

}
