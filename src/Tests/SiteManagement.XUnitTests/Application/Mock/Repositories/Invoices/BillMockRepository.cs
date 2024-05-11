using SiteManagement.Application.Mappings;
using SiteManagement.Application.Rules.Invoices.Bills;
using SiteManagement.Application.Services.Repositories.Invoices;
using SiteManagement.Domain.Entities.Invoices;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Invoices;
using SiteManagement.XUnitTests.Application.Mock.Repositories.Commons;
using SiteManagement.XUnitTests.Application.Mock.Rules.Buildings;

namespace SiteManagement.XUnitTests.Application.Mock.Repositories.Invoices;

public class BillMockRepository : BaseMockRepository<IBillReposiotry, Bill, SiteManagementMapingProfile, BillBusinessRules, BillFakeDatas>
{
    public BillMockRepository(BillFakeDatas fakeData) : base(fakeData)
    {
        BusinessRules = SetBusinessRules();   
    }

    public override BillBusinessRules SetBusinessRules()
    {
         return new BillBusinessRules(MockRepository.Object, MockApartmentBusinessRules.GetApartmentBusinessRules());
    }
}

