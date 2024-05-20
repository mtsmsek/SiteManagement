using Moq;
using SiteManagement.Application.Services.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.XUnitTests.Application.Mock.Services.Security
{
    public static class MockIUserOpreationClaimService
    {
        public static Mock<IUserOperationClaimService> GetIUserOperationClaimServiceInstance()
        {
            return new Mock<IUserOperationClaimService>();
        }
    }
}
