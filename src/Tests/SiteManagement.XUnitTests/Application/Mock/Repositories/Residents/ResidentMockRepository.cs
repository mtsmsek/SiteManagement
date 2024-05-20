using SiteManagement.Application.Mappings;
using SiteManagement.Application.Rules.Residents;
using SiteManagement.Application.Services.Repositories.Residents;
using SiteManagement.Domain.Entities.Residents;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Residents;
using SiteManagement.XUnitTests.Application.Mock.Repositories.Commons;

namespace SiteManagement.XUnitTests.Application.Mock.Repositories.Residents;

public class ResidentMockRepository : BaseMockRepository<IResidentRepository, Resident, SiteManagementMapingProfile, ResidentBusinessRules, ResidentFakeDatas>
{
    public ResidentMockRepository(ResidentFakeDatas fakeData) : base(fakeData)
    {
        BusinessRules = SetBusinessRules();
    }

    public override ResidentBusinessRules SetBusinessRules()
    {
        return new ResidentBusinessRules(MockRepository.Object);
    }
}
