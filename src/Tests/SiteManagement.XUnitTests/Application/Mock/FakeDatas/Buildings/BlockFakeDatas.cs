using SiteManagement.Domain.Entities.Buildings;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Commons;

namespace SiteManagement.XUnitTests.Application.Mock.FakeDatas.Buildings;

public class BlockFakeDatas : BaseFakeData<Block>
{
    public static Guid InDbId = Guid.NewGuid();
    public static Guid NotInDbId = GenerateNotInDbGuid();
    public static string InDbBlockName = "A";
    public static string NotInDbBlockName = "C";
    public static int TotalDataCount;

 
    public override List<Block> CreateFakeData()
    {
        var data = new List<Block>()
       {
           new()
           {
               Id = InDbId,
               Name = InDbBlockName,
               CreatedDate = DateTime.Now.AddDays(-5),

           },
           new()
           {
               Id= Guid.NewGuid(),
               Name = "B",
               CreatedDate= DateTime.Now.AddDays(-6),
           }
       };
        TotalDataCount = data.Count;
        return data;
    }
    private static Guid GenerateNotInDbGuid()
    {
        Guid notInDbId;
        do
        {
            notInDbId = Guid.NewGuid();
        } while (notInDbId == InDbId);

        return notInDbId;
    }
}
