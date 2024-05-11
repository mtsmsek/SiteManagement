using FluentValidation.Results;
using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;
using SiteManagement.Application.Features.Queries.Apartments.GetListAllApartmentsByBlock;
using SiteManagement.Application.Features.Queries.Invoices.GetListApartmentBillsByMonth;
using SiteManagement.Application.Pagination.Responses;
using SiteManagement.Application.Rules.Buildings.Blocks;
using SiteManagement.Domain.Constants.Buildings.Apartments;
using SiteManagement.Domain.Constants.Buildings.Blocks;
using SiteManagement.Domain.Entities.Buildings;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Buildings;
using SiteManagement.XUnitTests.Application.Mock.Repositories.Buildings;
using SiteManagement.XUnitTests.Application.Mock.Rules.Buildings;

namespace SiteManagement.XUnitTests.Application.Features.Buildings.Apartments.Queries;

public class GetListAllApartmentByBlockTests : ApartmentMockRepository
{
    private readonly GetListAllApartmentsByBlockQuery _query;
    private readonly GetListAllApartmentsByBlockQueryHandler _handler;
    private readonly BlockBusinessRules _blockBusinessRules;
    private readonly GetListAllApartmentsByBlockQueryValidator _validator;
    public GetListAllApartmentByBlockTests(ApartmentFakeDatas fakeData, GetListAllApartmentsByBlockQuery query, GetListAllApartmentsByBlockQueryValidator validator) : base(fakeData)
    {
        _blockBusinessRules = MockBlockBusinessRules.GetBlockBusinessRules();
        _query = query;
        _handler = new(MockRepository.Object, Mapper, _blockBusinessRules);
        _validator = validator;
    }

   
    [Fact]
    public void WhenBlockIdAndBlockNameEmpty_ShouldReturn_ValidationError()
    {
        //Arrange
        _query.BlockId = Guid.Empty;
        _query.BlockName = string.Empty;
        //Act
        ValidationFailure? result = _validator.Validate(_query).Errors.FirstOrDefault();
        //Assert
        Assert.Equal(ApartmentMessages.ValidationMessages.BlockIdAndBlockNameCannotBeEmptyAtTheSameTime, result?.ErrorMessage);
    }
    [Fact]
    public async Task WhenBlockIdIsNotEmptyButDoesNotExistInDatabase_ShouldReturn_BusinessException()
    {
        //Arrange
        _query.BlockId = Guid.NewGuid();
       
        //Act
        async Task Action() => await _handler.Handle(_query, CancellationToken.None);

        //Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(Action);

        Assert.Equal(BlockMessages.RuleMessages.BlocIsNotExist, exception.Message);


    }
    [Fact]
    public async Task WhenBlockNameIsNotEmptyButDoesNotExistInDatabase_ShouldReturn_BusinessException()
    {
       //Arrange
        _query.BlockName = BlockFakeDatas.NotInDbBlockName;
        
        //Act
        async Task Action() => await _handler.Handle(_query, CancellationToken.None);

        //Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(Action);

        Assert.Equal(BlockMessages.RuleMessages.BlocIsNotExist, exception.Message);


    }
    [Fact]
    public async Task WhenBlockNameIsNotEmptyAndExistsInDatabase_ShouldReturn_ResponseDatas()
    {
        //Arrange
        _query.BlockId = ApartmentFakeDatas.FirstBlockId;
        //Act
        var response = await _handler.Handle(_query, CancellationToken.None);
        //Assert
        Assert.NotNull(response);
        Assert.Equal(2, response.Results.Count);
        Assert.Equal("A", response.Results.FirstOrDefault()!.BlockName);


    }
}
