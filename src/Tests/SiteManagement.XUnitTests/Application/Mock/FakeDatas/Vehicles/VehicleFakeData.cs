using SiteManagement.Domain.Entities.Residents;
using SiteManagement.Domain.Entities.Vehicles;
using SiteManagement.Domain.Enumarations.Vehicles;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Buildings;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Commons;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Residents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.XUnitTests.Application.Mock.FakeDatas.Vehicles;

public class VehicleFakeData : BaseFakeData<Vehicle>
{
    public const string InDbRegistraionPlate = "34 AA 1212";
    public const string NotInDbRegistrationPlate = "34 BB 1212";
    public override List<Vehicle> CreateFakeData()
    {
        var data = new List<Vehicle>()
        {
             new Vehicle
             {
                 Id = InDbId,
                 CreatedDate = DateTime.Now,
                 VehicleRegistrationPlate = InDbRegistraionPlate,
                 VehicleType = VehicleType.Car,
                 Residents = new List<ResidentVehicle>()
                 {
                     new()
                     {
                         Id = Guid.NewGuid(),
                         CreatedDate = DateTime.Now,
                         ResidentId = ResidentFakeDatas.InDbId,
                         Resident = new Resident
                         {
                             Id= Guid.NewGuid(),
                             ApartmentId = ApartmentFakeDatas.InDbId,
                             Apartment = new()
                             {
                                 Id = ApartmentFakeDatas.InDbId,
                                 
                             }
                            
                         },
                         VehicleId = InDbId,
                         
                     }
                 }
             }
        };

        return data;
    }
}

