using FluentValidation.Results;
using Moq;
using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;
using SiteManagement.Application.Features.Commands.Invoices.Bills.CreateBill;
using SiteManagement.Application.Rules.Buildings.Apartments;
using SiteManagement.Domain.Constants.Buildings.Apartments;
using SiteManagement.Domain.Constants.Invoices.Bills;
using SiteManagement.Domain.Entities.Invoices;
using SiteManagement.Domain.Enumarations.Invoices;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Invoices;
using SiteManagement.XUnitTests.Application.Mock.Repositories.Invoices;
using SiteManagement.XUnitTests.Application.Mock.Rules.Buildings;

namespace SiteManagement.XUnitTests.Application.Features.Invoices.Bills.Commands.CreateBills;

public class CreateBillsTest : BillMockRepository
{
    private readonly CreateBillValidator _validator;
    private readonly CreateBillCommand _command;
    private readonly CreateBillCommandHandler _handler;
    private readonly ApartmentBusinessRules _apartmentBusinessRules;
    public CreateBillsTest(BillFakeDatas fakeData, CreateBillCommand command, CreateBillValidator validator) : base(fakeData)
    {
        _apartmentBusinessRules = MockApartmentBusinessRules.GetApartmentBusinessRules();
        _command = command;
        _handler = new(MockRepository.Object, Mapper, BusinessRules, _apartmentBusinessRules);
        _validator = validator;
    }
    [Fact]
    public void BillFeeLessThanOrEqualToZero_ShouldReturn_ValidationError()
    {
        //Arrange
        _command.Fee = 0;

        //Act
        ValidationFailure? response = _validator.Validate(_command)
                                       .Errors.FirstOrDefault();
        //Assert
        Assert.Equal(BillsMessages.ValidationMessages.BillFeeCannotBeLessThanOrEqualToZero, response?.ErrorMessage);

    }

    [Fact]
    public void YearIsNull_ShouldReturn_ValidationError()
    {
        //Arrange
        _command.Fee = 150;


        //Act
        ValidationFailure? response = _validator.Validate(_command)
                                     .Errors.FirstOrDefault();
        //Assert 
        Assert.Equal(BillsMessages.ValidationMessages.YearCannotBeNull, response?.ErrorMessage);
    }
    [Fact]
    public void YearValueIsGreaterThanCurrentYear_ShouldReturn_ValidationError()
    {
        //Arrange
        var currentTime = DateTime.Now;
        _command.Fee = 150;
        _command.Year = currentTime.Year + 1;


        //Act
        ValidationFailure? response = _validator.Validate(_command)
                                     .Errors.FirstOrDefault();
        //Assert 
        Assert.Equal(BillsMessages.ValidationMessages.YearValueShouldBeLessThanOrEqualToCurrentYearValue, response?.ErrorMessage);
    }

    [Fact]
    public void MonthIsNull_ShouldReturn_ValidationError()
    {
        //Arrange
        _command.Fee = 150;
        _command.Year = DateTime.Now.Year;

        //Act
        ValidationFailure? response = _validator.Validate(_command)
                                     .Errors.FirstOrDefault();
        //Assert 
        Assert.Equal(BillsMessages.ValidationMessages.MonthCannotBeNull, response?.ErrorMessage);

    }
    [Fact]
    public void MonthValueIsNotBetweenOneAndTwelve_ShouldReturn_ValidationError()
    {
        //Arrange
        _command.Fee = 150;
        _command.Year = DateTime.Now.Year;
        _command.Month = 13;

        //Act
        ValidationFailure? response = _validator.Validate(_command)
                                     .Errors.FirstOrDefault();
        //Assert 
        Assert.Equal(BillsMessages.ValidationMessages.MonthValueShouldBeBetweenOneAndTwelve, response?.ErrorMessage);
    }

    [Fact]
    public void MonthValueIsGreaterThanCurrentMonth_ShouldReturn_ValidationError()
    {

        //Arrange
        var currentDate = DateTime.Now;
        if (currentDate.Month == 12)
            currentDate.AddMonths(-1);

        _command.Fee = 150;
        _command.Year = currentDate.Year;
        _command.Month = currentDate.Month +1;
        _command.Type = 3;

        //Act
        ValidationFailure? response = _validator.Validate(_command)
                                     .Errors.FirstOrDefault();
        //Assert 
        Assert.Equal(BillsMessages.ValidationMessages.MonthValueShouldBeLessThanOrEqualToCurrentMonthValue, response?.ErrorMessage);
    }

    [Fact]
    public void InvalidBillType_ShouldReturn_ValidationError()
    {
        //Arrange
        var currentDate = DateTime.Now;

        _command.Fee = 150;
        _command.Year = currentDate.Year;
        _command.Month = currentDate.Month;
        _command.Type = BillType.Enumarations.Select(x => x.Value).LastOrDefault()!.Value + 1;
        //Act
        ValidationFailure? response = _validator.Validate(_command)
                             .Errors.FirstOrDefault();
        //Assert
        Assert.Equal(BillsMessages.ValidationMessages.InvalidBillType, response?.ErrorMessage);

    }



    [Fact]
    public async Task ApartmentDoesNotExistInDatabase_ShouldReturn_BusinessException()
    {
        //Arrange
        _command.ApartmentId = Guid.NewGuid();
        //Act
        Task Action() => _handler.Handle(_command, CancellationToken.None);
        //Assert
        var excetion = await Assert.ThrowsAsync<BusinessException>(Action);
        Assert.Equal(ApartmentMessages.RuleMessages.ApartmentCannotBeFound, excetion.Message);
    }

    [Fact]
    public async Task IncludingTwoBillTypesForTheSamePeriodAndApartment_ShouldReturn_BusinessException()
    {
        //Arrange
        _command.ApartmentId = BillFakeDatas.InDbApartmentGuid;
        _command.Year = 2024;
        _command.Month = 3;
        _command.Type = 1;
        _command.Fee = 50;
        //Act
        Task Action() => _handler.Handle(_command, CancellationToken.None);
        //Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(Action);
        Assert.Equal(BillsMessages.RuleMessages.ForTheSamePeriodAndApartmentCanIncludeOneBillType, exception.Message);

    }


    [Fact]
    public async Task CreatingBillSuccessfully_Should_CalledAddAsyncMethodOnce()
    {
        //Arrange
        _command.ApartmentId = BillFakeDatas.InDbApartmentGuid;
        _command.Year = 2024;
        _command.Month = 1;
        _command.Type = 3;
        _command.Fee = 50;

        //Act
         var response = await _handler.Handle(_command, CancellationToken.None);
        //Assert
        MockRepository.Verify(x => x.AddAsync(It.IsAny<Bill>(), It.IsAny<CancellationToken>()),Times.Once());
        Assert.NotEqual(Guid.Empty, response);
        
    }





}
