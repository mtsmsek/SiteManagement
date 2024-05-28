using SiteManagement.Application.Features.Queries.Residents.GetListAllResidents;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Residents;
using SiteManagement.XUnitTests.Application.Mock.Repositories.Residents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.XUnitTests.Application.Features.Residents.Queries
{
    public class GetListAllResidentTests : ResidentMockRepository
    {
        private readonly GetListAllResidentsQuery _query;
        private readonly GetListAllResidentsQueryHandler _handler;

        public GetListAllResidentTests(ResidentFakeDatas fakeData, GetListAllResidentsQuery query) : base(fakeData)
        {
            _query = query;
            _handler = new(MockRepository.Object, Mapper);
        }
        [Fact]
        public async Task GetListAllResidents_ShouldBring_TwoResidennts()
        {
            //Arrange

            //Act
            var response = await _handler.Handle(_query, CancellationToken.None);
            //Assert
            Assert.Equal(2 , response.Results.Count);
        }
    }
}
