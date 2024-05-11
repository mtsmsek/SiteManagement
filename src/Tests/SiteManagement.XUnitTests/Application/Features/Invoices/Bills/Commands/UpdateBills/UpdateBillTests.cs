using FluentValidation.Results;
using Moq;
using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;
using SiteManagement.Application.Features.Commands.Invoices.Bills.UpdateBill;
using SiteManagement.Domain.Constants.Invoices.Bills;
using SiteManagement.Domain.Entities.Invoices;
using SiteManagement.Domain.Enumarations.Invoices;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Invoices;
using SiteManagement.XUnitTests.Application.Mock.Repositories.Invoices;
using System.Reflection;

namespace SiteManagement.XUnitTests.Application.Features.Invoices.Bills.Commands.UpdateBills;

public class UpdateBillTests : BillMockRepository
{
    private readonly UpdateBillCommand _command;
    private readonly UpdateBillCommandHandler _handler;
    private readonly UpdateBillValidatior _validator;
    public UpdateBillTests(BillFakeDatas fakeData, UpdateBillCommand command, UpdateBillValidatior validator) : base(fakeData)
    {
        _command = command;
        _validator = validator;
        _handler = new(MockRepository.Object, Mapper, BusinessRules);
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
        _command.Month = currentDate.Month + 1;
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
        //TODO -- this test is the same as in the create bill tests, check if you can write it generic or not
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
    public async Task BillDoesNotExistInDatabase_ShouldReturn_BusinessException()
    {
        //Arrange
        _command.Id = BillFakeDatas.NotInDbId;
        //Act
        async Task Action() => await _handler.Handle(_command, CancellationToken.None);
        //Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(Action);
        Assert.Equal(BillsMessages.RuleMessages.BillCannotBeFoundInDb, exception.Message);
    }

    [Fact]
    public async Task UpdateBillSuccesfully_ShouldCall_UpdateAsyncOnce()
    {
        //Arrange
        var currentDate = DateTime.Now;
        _command.Id = BillFakeDatas.InDbId;
        _command.Fee = 150;
        _command.Year = currentDate.Year;
        _command.Month = currentDate.Month;
        _command.Type = BillType.Enumarations.Select(x => x.Value).LastOrDefault()!.Value;
        _command.IsPaid = true;
        var bill = new Bill
        {

            Id = BillFakeDatas.InDbId,
            Fee = 150,
            Year = currentDate.Year,
            Month = currentDate.Month,
            Type = BillType.NaturalGas,
            IsPaid = true
        };
        
        //Act
        var repsonse = await _handler.Handle(_command, CancellationToken.None);
        //Assert
        MockRepository.Verify(x => x.UpdateAsync(It.IsAny<Bill>(), CancellationToken.None), Times.Once());

        Assert.NotNull(repsonse);
        //TODO checking properties are same with the old values or not ??

    }


}
