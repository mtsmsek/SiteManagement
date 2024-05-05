using SiteManagement.Domain.Entities.Buildings;
using SiteManagement.Domain.Enumarations.Buildings;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Commons;

namespace SiteManagement.XUnitTests.Application.Mock.FakeDatas.Buildings;

public class ApartmentFakeDatas : BaseFakeData<Apartment>
{
    public override List<Apartment> CreateFakeData()
    {
        var datas = new List<Apartment>()
         {
            new()
            {
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                BlockId = Guid.NewGuid(),
                ApartmentNumber = 1,
                ApartmentType = ApartmentType.TwoPlusOne,
                FloorNumber = 1,
                IsTenant = true,
                Status = true,

            },
            new()
            {
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                BlockId = Guid.NewGuid(),
                ApartmentNumber = 2,
                ApartmentType = ApartmentType.TwoPlusOne,
                FloorNumber = 1,
                IsTenant = false,
                Status = true,
            },
            new()
            {
                       
            
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                BlockId = Guid.NewGuid(),
                ApartmentNumber = 3,
                ApartmentType = ApartmentType.ThreePlusOne,
                FloorNumber = 2,
                IsTenant = false,
                Status = false,
            
            }
        };

        return datas;
    }
}
