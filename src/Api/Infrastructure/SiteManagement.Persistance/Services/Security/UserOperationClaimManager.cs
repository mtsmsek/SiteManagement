using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;
using SiteManagement.Application.Services.Repositories.Security;
using SiteManagement.Application.Services.Security;
using SiteManagement.Domain.Entities.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Persistance.Services.Security
{
    public class UserOperationClaimManager : IUserOperationClaimService
    {
        private readonly IUserOperationClaimRepository _userOperationClaimRepository;
        private readonly IOperationClaimRepository _operationClaimRepository;

        public UserOperationClaimManager(IUserOperationClaimRepository userOperationClaimRepository, IOperationClaimRepository operationClaimRepository)
        {
            _userOperationClaimRepository = userOperationClaimRepository;
            _operationClaimRepository = operationClaimRepository;
        }

        public async Task AddUserWithOperationClaim(Guid id, string operationClaimName)
        {
            var operationClaim = await _operationClaimRepository.GetSingleAsync(predicate: o => o.Name == operationClaimName);

            //todo -- remove magic string
            if (operationClaim is null)
                throw new BusinessException("Eklemek istediğiniz operationClaim bulunamadı");

            var userOperationClaimToAdd = new UserOperationClaim()
            {
                UserId = id,
                OperationClaimId = operationClaim.Id
            };

            await _userOperationClaimRepository.AddAsync(userOperationClaimToAdd);
        }
    }
}
