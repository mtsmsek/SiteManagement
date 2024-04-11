using AutoMapper;
using MediatR;
using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;
using SiteManagement.Application.Services.Repositories.Security;
using SiteManagement.Domain.Entities.Security;

namespace SiteManagement.Application.Features.Commands.Security.OperationClaims.UpdateOperationClaim
{
    public class UpdateOperationClaimCommandHandler : IRequestHandler<UpdateOperationClaimCommand, UpdateOperationClaimResponse>
    {
        private readonly IOperationClaimRepository _operationClaimRepository;
        private readonly IMapper _mappper;

        public UpdateOperationClaimCommandHandler(IOperationClaimRepository operationClaimRepository, IMapper mappper)
        {
            _operationClaimRepository = operationClaimRepository;
            _mappper = mappper;
        }

        public async Task<UpdateOperationClaimResponse> Handle(UpdateOperationClaimCommand request, CancellationToken cancellationToken)
        {
            var opearationClaim = await _operationClaimRepository.GetByIdAsync(id: request.Id, cancellationToken: cancellationToken);
            if (opearationClaim is null)
                throw new BusinessException("Operation claim cannot be found");

            var operationClaimToUpdate = _mappper.Map(request, opearationClaim);

            return _mappper.Map<UpdateOperationClaimResponse>(operationClaimToUpdate);


        }
    }
}
