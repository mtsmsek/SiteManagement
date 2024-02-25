using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Handlers;
using SiteManagement.Application.CrossCuttingConcerns.Exceptions.HttpProblemDetails;
using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Middlewares;
using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;
using SiteManagement.Application.Features.Commands.Buildings.Blocks.CreateBlock;
using SiteManagement.Application.Mappings;
using SiteManagement.Application.Rules.Buildings.Blocks;
using SiteManagement.Application.Services.Repositories.Buildings;
using SiteManagement.Domain.Constants.Buildings.Blocks;
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
        public async Task CreateBlockHandler_ShouldCallAddAsyncOnce_WhenBlockNameIsUnique()
        {
            //Arrange
            var middleware = new Mock<ExceptionMiddleware>();   
            var blockRepositoryMock = new Mock<IBlockRepository>();
            var blockBusinessMock = new Mock<BlockBusinessRules>(blockRepositoryMock.Object);
            var middlewareMock = new Mock<ExceptionMiddleware>();


            var command =  new CreateBlockCommand();
            var mapperConfig = new MapperConfiguration(c =>
            {
                c.AddProfile<SiteManagementMapingProfile>();
            });
           
            IMapper mapper = mapperConfig.CreateMapper();

            var exception = new BusinessException();
            var handler = new CreateBlockCommandHandler(blockRepositoryMock.Object, mapper, blockBusinessMock.Object);

            //blockBusinessMock
            //.Setup(i => i.BlockNameCannotBeDublicateWhenAddOrUpdate(It.IsAny<string>(), It.IsAny<string>()));

           blockRepositoryMock.Setup(i => i.IsBlockNameUnique(It.IsAny<string>()))
           .ReturnsAsync(false);
            
            
            //Act
            var result = await handler.Handle(command, default);

            //Assert
            
            middleware.Verify(i => i.Invoke(It.IsAny<HttpContext>()));
           // context.Response.Should().Be(exception);
            
          //  middleware.Should().Be(middleware,BlockMessages.RuleMessages.BlocIsNotExist);
           // middleware.Setup(i => i.Invoke(It.IsAny<HttpContext>())).Returns(exception);



        }



    }
}
