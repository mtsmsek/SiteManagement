using SiteManagement.Domain.Entities.Invoices;
using SiteManagement.Domain.Enumarations.Invoices;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Buildings;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Commons;

namespace SiteManagement.XUnitTests.Application.Mock.FakeDatas.Invoices
{
    public class BillFakeDatas : BaseFakeData<Bill>
    {
        public static Guid InDbApartmentGuid = ApartmentFakeDatas.InDbId;
        public override List<Bill> CreateFakeData()
        {
            var list = new List<Bill>()
            {
                //TODO -- add apartmnet and block datas manually??
                new()
                {
                    Id = InDbId,
                    CreatedDate = DateTime.Now.AddMonths(-1),
                    ApartmentId = InDbApartmentGuid,
                    Fee = 500,
                    IsPaid = true,
                    Month = Month.March,
                    Year = 2024,
                    Type = BillType.Electricity,

                },
                new()
                {
                    Id = Guid.NewGuid(),
                    CreatedDate = DateTime.Now.AddMonths(-1),
                    ApartmentId = InDbApartmentGuid,
                    Fee = 400,
                    IsPaid = false,
                    Month = Month.February,
                    Year = 2024,
                    Type = BillType.WaterBill,

                },
                new()
                {
                    Id = Guid.NewGuid(),
                    CreatedDate = DateTime.Now.AddMonths(-2),
                    ApartmentId = Guid.NewGuid(),
                    Fee = 600,
                    IsPaid = false,
                    Month = Month.May,
                    Year = 2024,
                    Type = BillType.NaturalGas,

                }

            };
            return list;
        }
    }
}
