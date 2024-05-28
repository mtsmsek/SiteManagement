using Moq;
using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;
using SiteManagement.Application.Features.Queries.Residents.GetListResidentByApartmentNumberAndBlockName;
using SiteManagement.Application.Services.Repositories.Buildings;
using SiteManagement.Domain.Constants.Buildings.Blocks;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Buildings;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Residents;
using SiteManagement.XUnitTests.Application.Mock.Repositories.Residents;
using SiteManagement.XUnitTests.Application.Mock.Rules.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.XUnitTests.Application.Features.Residents.Queries
{
    public class GetListResidentByApartmentNumberAndBlockNameTests : ResidentMockRepository
    {
        private readonly GetListResidentsByApartmentNumberAndBlockNameQuery _query;
        private readonly GetListResidentsByApartmentNumberAndBlockNameQueryHandler _handler;
        public GetListResidentByApartmentNumberAndBlockNameTests(ResidentFakeDatas fakeData, GetListResidentsByApartmentNumberAndBlockNameQuery query) : base(fakeData)
        {
            var blockBusinessRules = MockBlockBusinessRules.GetBlockBusinessRules();
            var apartmentRepository = new Mock<IApartmentRepository>();
            _query = query;
            _handler = new(MockRepository.Object, Mapper, blockBusinessRules, apartmentRepository.Object);
        }
        [Fact]
        public async Task BlockDoesNotExistInDb_ShouldReturn_BusinessException()
        {
            //Act
            _query.BlockName = BlockFakeDatas.NotInDbBlockName;
            _query.ApartmentNumber = 88;
            //Action
            Task Action() => _handler.Handle(_query, CancellationToken.None);
            //Assert
            var response = await Assert.ThrowsAsync<BusinessException>(Action);
            Assert.Equal(BlockMessages.RuleMessages.BlocIsNotExist, response.Message);
        }
        [Fact]
        public async Task BlockExistsAndApartmentNumberTwo_ShouldReturn_OneResident()
        {
            //Act
            _query.BlockName = BlockFakeDatas.InDbBlockName;
            _query.ApartmentNumber = 1;
            //Action
            var response = await _handler.Handle(_query, CancellationToken.None);
            //Assert
            Assert.Equal(1, response.Results.Count);
        }
    }
}
