using FluentValidation.Results;
using SiteManagement.Application.Features.Commands.Buildings.Blocks.CreateBlock;
using SiteManagement.XUnitTests.Mock.FakeDatas.Buildings;
using SiteManagement.XUnitTests.Mock.Repositories.Buildings;
using SiteManagement.Domain.Constants.Buildings.Blocks;
using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;
using Moq;
using SiteManagement.Domain.Entities.Buildings;
using Microsoft.AspNetCore.Authorization;

namespace SiteManagement.XUnitTests.Features.Buildings.Blocks.Commands.CreateBlock;

public class CreateBlockTests : BlockMockRepository
{
    private readonly CreateBlockCommandValidator _validator;
    private readonly CreateBlockCommand _command;
    private readonly CreateBlockCommandHandler _handler;

    public CreateBlockTests(BlockFakeDatas fakeDatas, CreateBlockCommandValidator validator, CreateBlockCommand command) : base(fakeDatas)
    {
        _validator = validator;
        _command = command;
        _handler = new CreateBlockCommandHandler(MockRepository.Object, Mapper, BusinessRules);

    }
    #region Validation
    [Fact]
    public void BlockNameLongerThanTwoCharacters_ShouldReturn_ValidationError()
    {
        _command.Name = "ABC";
        //check first or default => x => x.PropertyName
        ValidationFailure? result = _validator.Validate(_command)
            .Errors.FirstOrDefault();

        Assert.Equal(BlockMessages.ValidationMessages.BlockNameCannotBeLongerThanTwoCharacters, result?.ErrorMessage);
    }
    [Fact]
    public void BlockNameEmpty_ShouldReturn_ValidationError()
    {
        _command.Name = string.Empty;
        ValidationFailure? result = _validator.Validate(_command)
            .Errors.FirstOrDefault();

        Assert.Equal(BlockMessages.ValidationMessages.BlockNameCannotBeEmpty, result?.ErrorMessage);
    }
    [Fact]
    public void BlockNameStartsWithNumber_ShouldReturn_ValidationError()
    {
        _command.Name = "1A";
        ValidationFailure? result = _validator.Validate(_command)
            .Errors.FirstOrDefault();

        Assert.Equal(BlockMessages.ValidationMessages.BlockNameMustStartWithALetter, result?.ErrorMessage);
    }
    #endregion

    [Fact]
    public async Task BlockNameCanotInsertedWhenAdding_ShouldReturn_BusinessException()
    {

        _command.Name = "A";
        async Task Action() => await _handler.Handle(_command, CancellationToken.None);

        var exception = await Assert.ThrowsAsync<BusinessException>(Action);
        Assert.Equal(BlockMessages.RuleMessages.BlockNameAlreadyExist, exception?.Message);
    }

    [Fact]
    public async Task CreateBlockSuccessfully_Should_CallAddAsyncOnce()
    {
        _command.Name = "C";

        await _handler.Handle(_command, CancellationToken.None);

        MockRepository.Verify(s => s.AddAsync(It.IsAny<Block>(),CancellationToken.None), Times.Once); 
    }
}







