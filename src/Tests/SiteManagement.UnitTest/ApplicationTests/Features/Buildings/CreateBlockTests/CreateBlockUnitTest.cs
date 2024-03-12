using AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SiteManagement.Api.WebApi.Controllers.Buildings;
using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Handlers;
using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;
using SiteManagement.Application.Features.Commands.Buildings.Blocks.CreateBlock;
using SiteManagement.Application.Mappings;
using SiteManagement.Application.Rules.Buildings.Blocks;
using SiteManagement.Application.Services.Repositories.Buildings;

namespace SiteManagement.UnitTest.ApplicationTests.Features.Buildings.CreateBlockTests
{
    [TestClass]
    public class CreateBlockUnitTest
    {
        private readonly MapperConfiguration _mapperConfig;
        private readonly Mock<IBlockRepository> _blockRepositoryMock;
        private readonly Mock<BlockBusinessRules> _blockBusinessRulesMock;
        private readonly IMapper _mapper;
        private readonly BlocksController _controller;
        public CreateBlockUnitTest()
        {
            _blockRepositoryMock = new();
            
            _blockBusinessRulesMock = new Mock<BlockBusinessRules>(_blockRepositoryMock.Object);
           var _mapperConfig = new MapperConfiguration(c =>
            {
                c.AddProfile<SiteManagementMapingProfile>();
            });
            _mapper = _mapperConfig.CreateMapper();
            _controller = new();
        }

        [Test]
        public async Task CreateBlockHandler_Return_EmptyGuid_WhenBlockNameIsNotUnique()
        {
            //Arrange
            var exception = new BusinessException();
            var command = new CreateBlockCommand()
            {
                Name = "a"
            };

           var m = new Mock<ExceptionHandler>();
            //var m = new HttpExceptionHandler();

            var handler = new CreateBlockCommandHandler(_blockRepositoryMock.Object, _mapper, _blockBusinessRulesMock.Object);

            _blockBusinessRulesMock
                .Setup(i => i.BlockNameCannotBeDublicateWhenAddOrUpdate(It.IsAny<string>(), It.IsAny<string>()));

            _blockRepositoryMock.Setup(i => i.IsBlockNameUnique(It.IsAny<string>()))
            .ReturnsAsync(false);


            //Act
            var result = await handler.Handle(command, default);

            //Assert


           
            //result.Should().Be(Guid.Empty);
       




        }



    }
}
