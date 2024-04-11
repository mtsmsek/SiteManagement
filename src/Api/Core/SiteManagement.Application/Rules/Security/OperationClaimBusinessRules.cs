using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;
using SiteManagement.Application.Rules.Commons;
using SiteManagement.Application.Services.Repositories.Security;

namespace SiteManagement.Application.Rules.Security
{
    public class OperationClaimBusinessRules : BaseBusinessRules
    {
        private readonly IOperationClaimRepository _operationClaimRepository;

        public OperationClaimBusinessRules(IOperationClaimRepository operationClaimRepository)
        {
            _operationClaimRepository = operationClaimRepository;
        }

        public async Task OperationClaimNameCannotBeDuplicateWhenInsert(string name, CancellationToken cancellationToken)
        {
            var operationClaim = await _operationClaimRepository.GetSingleAsync(predicate: oc => oc.Name == name,
                                                           cancellationToken: cancellationToken);

            //todo -- remove megic string
            if (operationClaim is not null)
                throw new BusinessException("Opearation claim name cannot be duplicate!");


        }
    }
}
