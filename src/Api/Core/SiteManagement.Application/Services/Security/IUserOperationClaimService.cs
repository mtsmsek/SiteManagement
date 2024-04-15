using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Services.Security
{
    public interface IUserOperationClaimService
    {
        public Task AddUserWithOperationClaim(Guid id, string operationClaimName);

    }
}
