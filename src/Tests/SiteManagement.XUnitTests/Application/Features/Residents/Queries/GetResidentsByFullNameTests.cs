using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;
using SiteManagement.Application.Features.Queries.Residents.GetResidentsByFullName;
using SiteManagement.Domain.Constants.Residents;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Residents;
using SiteManagement.XUnitTests.Application.Mock.Repositories.Residents;

namespace SiteManagement.XUnitTests.Application.Features.Residents.Queries;

public class GetResidentsByFullNameTests : ResidentMockRepository
{
    private readonly GetResidentsByFullNameQuery _query;
    private readonly GetResidentsByFullNameQueryHandler _handler;
    public GetResidentsByFullNameTests(ResidentFakeDatas fakeData, GetResidentsByFullNameQuery query) : base(fakeData)
    {
        _query = query;
        _handler = new(MockRepository.Object, Mapper);
    }
    [Fact]
    public async Task NoResidentWithName_ShouldReturn_BusinessException()
    {
        //Arrange
        _query.FirstName = "Mehmet";
        _query.LastName = "Şimşek";
       
        //Act
        async Task Action() => await _handler.Handle(_query, CancellationToken.None);

        //Assert 
        var response = await Assert.ThrowsAsync<BusinessException>(Action);
        Assert.Equal(ResidentMessages.RuleMessages.ResidentCannotBeFound, response.Message);
    }
    [Fact]
    public async Task ResidentFullNameExistsInDb_ShouldReturn_AtLeastOneResultCount()
    {
        //Arrange
        _query.FirstName = "Test";
        _query.LastName = "Test";

        //Act
        var resonse =  await _handler.Handle(_query, CancellationToken.None);

        //Assert 
        Assert.Equal(1, resonse.Results.Count);
    }
}
