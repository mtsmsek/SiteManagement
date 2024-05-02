﻿using SiteManagement.Domain.Entities.Buildings;
using SiteManagement.XUnitTests.Mock.FakeDatas.Commons;

namespace SiteManagement.XUnitTests.Mock.FakeDatas.Buildings;

public class BlockFakeDatas : BaseFakeData<Block>
{
    public override List<Block> CreateFakeData()
    {
        var data = new List<Block>()
       {
           new()
           {
               Id = Guid.NewGuid(),
               Name = "A",
               CreatedDate = DateTime.Now.AddDays(-5),

           },
           new()
           {
               Id= Guid.NewGuid(),
               Name = "B",
               CreatedDate= DateTime.Now.AddDays(-6),
           }
       };
        return data;
    }
}