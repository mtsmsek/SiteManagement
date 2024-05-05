using Moq;
using SiteManagement.Application.Mappings;
using SiteManagement.Application.Rules.Buildings.Apartments;
using SiteManagement.Application.Rules.Buildings.Blocks;
using SiteManagement.Application.Services.Repositories.Buildings;
using SiteManagement.Domain.Entities.Buildings;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Buildings;
using SiteManagement.XUnitTests.Application.Mock.Repositories.Commons;

namespace SiteManagement.XUnitTests.Application.Mock.Repositories.Buildings;

public class ApartmentMockRepository : BaseMockRepository<IApartmentRepository, Apartment, SiteManagementMapingProfile, ApartmentBusinessRules, ApartmentFakeDatas>
{
    public ApartmentMockRepository(ApartmentFakeDatas fakeData) : base(fakeData)
    {

        BusinessRules = SetBusinessRules();
    }

    public override ApartmentBusinessRules SetBusinessRules()
    {
         return  new ApartmentBusinessRules(MockRepository.Object, new Mock<IBlockRepository>().Object);
    }
}
