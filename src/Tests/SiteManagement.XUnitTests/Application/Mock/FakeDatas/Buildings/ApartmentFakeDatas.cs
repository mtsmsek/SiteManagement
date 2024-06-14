using SiteManagement.Domain.Entities.Buildings;
using SiteManagement.Domain.Enumarations.Buildings;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Commons;

namespace SiteManagement.XUnitTests.Application.Mock.FakeDatas.Buildings;

public class ApartmentFakeDatas : BaseFakeData<Apartment>
{

    public static Guid FirstBlockId = BlockFakeDatas.InDbId;

    public override List<Apartment> CreateFakeData()
    {
        var datas = new List<Apartment>()
         {
            new()
            {
                Id = InDbId,
                CreatedDate = DateTime.Now,
                BlockId = FirstBlockId,
                ApartmentNumber = 1,
                ApartmentType = ApartmentType.TwoPlusOne,
                FloorNumber = 1,
                IsTenant = true,
                Status = true,
                Block = new Block()
                {
                    Id = FirstBlockId,
                    Name = "A",
                }


            },
            new()
            {
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                BlockId = FirstBlockId,
                ApartmentNumber = 2,
                ApartmentType = ApartmentType.TwoPlusOne,
                FloorNumber = 1,
                IsTenant = false,
                Status = true,
                Block = new Block()
                {
                    Id = FirstBlockId,
                    Name = "A",
                }
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
                Block = new Block()
                {
                    Id = Guid.NewGuid(),
                    Name = "B",
                }


            }
        };

        return datas;
    }
}
