using SiteManagement.Application.Features.Queries.Apartments.GetListApartmentsByStatus;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Buildings;
using SiteManagement.XUnitTests.Application.Mock.Repositories.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.XUnitTests.Application.Features.Buildings.Apartments.Queries;

public class GetListApartmentsByStatusTests : ApartmentMockRepository
{
    private readonly GetListApartmentsByStatusQuery _query;
    private readonly GetListApartmentsByStatusQueryHandler _handler;
    public GetListApartmentsByStatusTests(ApartmentFakeDatas fakeData, GetListApartmentsByStatusQuery query) : base(fakeData)
    {
        _query = query;
        _handler = new(MockRepository.Object, Mapper);
    }
    [Fact]
    public async Task TrueStatus_ResponseShould_Return_SameNumberOfDataAsTheInTheDatabase()
    {
        //Arrange
        _query.Status = true;
        //Act
        var response = await _handler.Handle(_query, CancellationToken.None);
        //Assert
        Assert.Equal(2, response.Results.Count);
    }
    [Fact]
    public async Task FalseStatus_ResponseShould_Return_SameNumberOfDataAsTheInTheDatabase()
    {
        //Arrange
        _query.Status = false;
        //Act
        var response = await _handler.Handle(_query, CancellationToken.None);
        //Assert
        Assert.Equal(1, response.Results.Count);
    }

}
