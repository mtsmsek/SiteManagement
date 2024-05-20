using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;
using SiteManagement.Application.Features.Queries.Invoices.GetListApartmentBillsByMonth;
using SiteManagement.Domain.Constants.Buildings.Apartments;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Invoices;
using SiteManagement.XUnitTests.Application.Mock.Repositories.Invoices;
using SiteManagement.XUnitTests.Application.Mock.Rules.Buildings;

namespace SiteManagement.XUnitTests.Application.Features.Invoices.Bills.Queries;

public class GetListApartmentBillsTests : BillMockRepository
{
    private readonly GetListApartmentBillsQuery _query;
    private readonly GetListApartmentBillsQueryHandler _handler;

    public GetListApartmentBillsTests(BillFakeDatas fakeData, GetListApartmentBillsQuery query) : base(fakeData)
    {
        var apartmentBusinessRules = MockApartmentBusinessRules.GetApartmentBusinessRules();
        _query = query;
        _handler = new(MockRepository.Object, Mapper, BusinessRules, apartmentBusinessRules);
    }
    

    [Fact]
    public async Task ApartmentDoesNotExistInDb_ShouldReturn_BusinessException()
    {
        //Arrange
        _query.ApartmentId = Guid.NewGuid();

        //Act
        async Task Action() => await _handler.Handle(_query, CancellationToken.None);

        //Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(Action);
        Assert.Equal(ApartmentMessages.RuleMessages.ApartmentCannotBeFound, exception.Message);
    }

    [Fact]
    public async Task InDbApartmentGuidMarch2024Electricity_ShouldReturn_OneBill()
    {
        //Arrange
        _query.ApartmentId = BillFakeDatas.InDbApartmentGuid;
        _query.Month = 3;
        _query.Year = 2024;
        _query.BillType = 1;
        //Act
        var response = await _handler.Handle(_query, CancellationToken.None);

        //Assert
        Assert.Equal(1, response.Results.Count);
    }
    [Fact]
    public async Task InDbApartmentGuidMarch2022Electricity_ShouldReturn_ZeroBill()
    {
        //Arrange
        _query.ApartmentId = BillFakeDatas.InDbApartmentGuid;
        _query.Month = 3;
        _query.Year = 2022;
        _query.BillType = 1;
        //Act
        var response = await _handler.Handle(_query, CancellationToken.None);

        //Assert
        Assert.Equal(0, response.Results.Count);
    }
    [Fact]
    public async Task InDbApartmentGuidMarchElectricityWithNullYearValue_ShouldReturn_TwoBill()
    {
        //Arrange
        _query.ApartmentId = BillFakeDatas.InDbApartmentGuid;
        _query.Month = 3;
        _query.BillType = 1;
        //Act
        var response = await _handler.Handle(_query, CancellationToken.None);

        //Assert
        Assert.Equal(2, response.Results.Count);
    }
    [Fact]
    public async Task InDbApartmentGuidElectricityWithNullYearAndMonthValue_ShouldReturn_ThreeBill()
    {
        //Arrange
        _query.ApartmentId = BillFakeDatas.InDbApartmentGuid;
        _query.BillType = 1;
        //Act
        var response = await _handler.Handle(_query, CancellationToken.None);

        //Assert
        Assert.Equal(3, response.Results.Count);
    }


}
