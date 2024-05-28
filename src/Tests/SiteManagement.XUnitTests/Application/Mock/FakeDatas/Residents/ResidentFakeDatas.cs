using SiteManagement.Application.Security.Hashing;
using SiteManagement.Domain.Entities.Residents;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Buildings;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Commons;

namespace SiteManagement.XUnitTests.Application.Mock.FakeDatas.Residents;

public class ResidentFakeDatas : BaseFakeData<Resident>
{
    public const string InDbEmail = "mehmet@gmail.com";
    public const string NotInDbEmail = "test@gmail.com";
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
                IdenticalNumber = "10987654321",
                PhoneNumber = "1234567890",
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
              
                
                
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


            }
        };


        return datas;
    }
   
}
