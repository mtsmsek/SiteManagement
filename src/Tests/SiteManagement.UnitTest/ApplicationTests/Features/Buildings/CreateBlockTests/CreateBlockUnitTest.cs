using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SiteManagement.Application.Features.Commands.Buildings.Blocks.CreateBlock;
using SiteManagement.Application.Rules.Buildings.Blocks;
using SiteManagement.Application.Services.Repositories.Buildings;
using SiteManagement.Domain.Entities.Buildings;
using SiteManagement.Persistance.Services.Repositories.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.UnitTest.ApplicationTests.Features.Buildings.CreateBlockTests
{
    [TestClass]
    public class CreateBlockUnitTest
    {
        [Test]
        public void CreateBlock_ThrowNewBusinessException_WithNullEntity()
        {
            //Arrange
            var blockRepositoryMock = new Mock<IBlockRepository>();
            var blockBusinessRules = new Mock<BlockBusinessRules>();
            var handleMock = new Mock<CreateBlockCommandHandler>();

            var a = blockRepositoryMock.Setup(i => i.IsBlockExist(It.IsAny<Guid>())).Returns<Block>(default);
            
          
         
            
            
            //Act

            //Assert
        }

    }
}
