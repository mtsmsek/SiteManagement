using AutoMapper;
using MediatR;
using SiteManagement.Application.Pipelines.Authorization;
using SiteManagement.Application.Rules.Security;
using SiteManagement.Application.Services.Repositories.Security;
using SiteManagement.Domain.Constants.Security;
using SiteManagement.Domain.Entities.Security;
using static SiteManagement.Domain.Constants.Security.UsersOperationClaims;

namespace SiteManagement.Application.Features.Commands.Security.OperationClaims.CreateOperationClaim
{
    public class CreateOperationClaimCommandHandler : IRequestHandler<CreateOperationClaimCommand, CreateOperationClaimResponse>, ISecuredRequest
    {
        private readonly IOperationClaimRepository _operationClaimRepository;
        private readonly IMapper _mapper;
        private readonly OperationClaimBusinessRules _operationClaimBusinessRules;

        public CreateOperationClaimCommandHandler(IOperationClaimRepository operationClaimRepository, IMapper mapper, OperationClaimBusinessRules operationClaimBusinessRules)
        {
            _operationClaimRepository = operationClaimRepository;
            _mapper = mapper;
            _operationClaimBusinessRules = operationClaimBusinessRules;
        }

        public string[] Roles => [Admin, Add];

        public async Task<CreateOperationClaimResponse> Handle(CreateOperationClaimCommand request, CancellationToken cancellationToken)
        {
            await _operationClaimBusinessRules.OperationClaimNameCannotBeDuplicateWhenInsert(request.Name, cancellationToken);

            var operationClaimToAdd = _mapper.Map<OperationClaim>(request);

            await _operationClaimRepository.AddAsync(operationClaimToAdd);

            return _mapper.Map<CreateOperationClaimResponse>(operationClaimToAdd);
        }
    }
}
