using Moq;
using SiteManagement.Application.Rules.Residents;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Residents;
using SiteManagement.XUnitTests.Application.Mock.Repositories.Residents;

namespace SiteManagement.XUnitTests.Application.Mock.Rules.Residents;

public static class MockResidentBusinessRules
{
    public static ResidentBusinessRules GetResidentBusinessRules()
    {
        var residentMockRepository = new ResidentMockRepository(new ResidentFakeDatas());


        var mockBusinessRule = new Mock<ResidentBusinessRules>(residentMockRepository.MockRepository.Object);
        return mockBusinessRule.Object;

    }
}
