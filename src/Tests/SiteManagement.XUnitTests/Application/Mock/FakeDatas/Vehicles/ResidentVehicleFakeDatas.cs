using SiteManagement.Domain.Entities.Vehicles;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Commons;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Residents;

namespace SiteManagement.XUnitTests.Application.Mock.FakeDatas.Vehicles;

public class ResidentVehicleFakeDatas : BaseFakeData<ResidentVehicle>
{
    public override List<ResidentVehicle> CreateFakeData()
    {
        //todo create fake data
        var data = new List<ResidentVehicle>()
        {
            new()
            {
                Id = InDbId,
                CreatedDate = DateTime.Now,
                DriveStatus = true,
                ResidentId = ResidentFakeDatas.InDbId,
                VehicleId = VehicleFakeData.InDbId,
                Resident = new()
                {
                    Id = ResidentFakeDatas.InDbId
                },
                Vehicle = new()
                {
                    Id = VehicleFakeData.InDbId,

                }


            }

        };


        return data;
    }
}
