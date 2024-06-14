using SiteManagement.Application.Security.Hashing;
using SiteManagement.Domain.Entities.Residents;
using SiteManagement.Domain.Entities.Vehicles;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Buildings;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Commons;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Vehicles;

namespace SiteManagement.XUnitTests.Application.Mock.FakeDatas.Residents;

public class ResidentFakeDatas : BaseFakeData<Resident>
{
    public const string InDbEmail = "mehmet@gmail.com";
    public const string NotInDbEmail = "test@gmail.com";

    public const string InDbIdenticalNumber = "10987654321";
    public static Guid Under18ResidentId = Guid.NewGuid();
    public override List<Resident> CreateFakeData()
    {
        HashingHelper.CreatePasswordHash("12345", out byte[] passwordHash, out byte[] passwordSalt);
        var guid = Guid.NewGuid();
        var datas = new List<Resident>()
        {
            new()
            {
                Id = InDbId,
                CreatedDate = DateTime.Now,
                ApartmentId = ApartmentFakeDatas.InDbId,
                BirthDate = DateTime.Now.AddYears(-27),
                Email = InDbEmail,
                FirstName = "Test",
                LastName = "Test",
                IdenticalNumber = InDbIdenticalNumber,
                PhoneNumber = "1234567890",
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Apartment = new()
                {
                    Id = ApartmentFakeDatas.InDbId,
                    ApartmentNumber = 1,
                    BlockId = BlockFakeDatas.InDbId,
                    Block = new()
                    {
                        Id = BlockFakeDatas.InDbId,
                        Name = BlockFakeDatas.InDbBlockName
                    }
                },
                Vehicles = new List<ResidentVehicle>()
                {
                    new()
                    {
                        Id = Guid.NewGuid(),
                        CreatedDate = DateTime.Now,
                        DriveStatus = true,
                        VehicleId = VehicleFakeData.InDbId,
                        ResidentId = InDbId,
                        Vehicle = new()
                        {
                            Id = VehicleFakeData.InDbId,
                            VehicleRegistrationPlate = VehicleFakeData.InDbRegistraionPlate,
                        }
                     
                    }
                }
              
                
                
            },
            new() {
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                ApartmentId = ApartmentFakeDatas.InDbId,
                BirthDate = DateTime.Now.AddYears(-28),
                Email = "ahmet@gmail.com",
                FirstName = "Test2",
                LastName = "Test2",
                IdenticalNumber = "12345123456",
                PhoneNumber = "12345789",
                Apartment = new()
                {
                    Id = ApartmentFakeDatas.InDbId,
                    ApartmentNumber = 2,
                    BlockId = BlockFakeDatas.InDbId,
                    Block = new()
                    {
                        Id = BlockFakeDatas.InDbId,
                        Name = BlockFakeDatas.InDbBlockName
                    },


                },
                Vehicles = new List<ResidentVehicle>()
                {
                    new()
                    {
                        Id = Guid.NewGuid(),
                        CreatedDate = DateTime.Now,
                        DriveStatus = true,
                        VehicleId = VehicleFakeData.NotInDbId,
                        ResidentId = Guid.NewGuid(),

                        Vehicle = new()
                        {
                            Id = VehicleFakeData.InDbId,
                            VehicleRegistrationPlate = "111",
                        }
                    }
                }

            },
            //for create vehicle test
            new() {
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                ApartmentId = Under18ResidentId,
                BirthDate = DateTime.Now.AddYears(-28),
                Email = "ahmet@gmail.com",
                FirstName = "Test2",
                LastName = "Test2",
                IdenticalNumber = "12345123456",
                PhoneNumber = "12345789",
                Apartment = new()
                {
                    Id = Under18ResidentId,
                    ApartmentNumber = 2,
                    BlockId = BlockFakeDatas.InDbId,
                    Block = new()
                    {
                        Id = BlockFakeDatas.InDbId,
                        Name = BlockFakeDatas.InDbBlockName
                    },


                },
                Vehicles = new List<ResidentVehicle>()
                {
                    new()
                    {
                        Id = Guid.NewGuid(),
                        CreatedDate = DateTime.Now,
                        DriveStatus = true,
                        VehicleId = VehicleFakeData.NotInDbId,
                        ResidentId = Guid.NewGuid(),

                        Vehicle = new()
                        {
                            Id = VehicleFakeData.InDbId,
                            VehicleRegistrationPlate = "111",
                        }
                    }
                }

            }
        };


        return datas;
    }
   
}
