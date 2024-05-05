using FluentValidation.Results;
using Moq;
using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;
using SiteManagement.Application.Features.Commands.Buildings.Blocks.UpdateBlock.UpdateBlockName;
using SiteManagement.Domain.Constants.Buildings.Blocks;
using SiteManagement.Domain.Entities.Buildings;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Buildings;
using SiteManagement.XUnitTests.Application.Mock.Repositories.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.XUnitTests.Application.Features.Buildings.Blocks.Commands.UpdateBlock;

public class UpdateBlockTests : BlockMockRepository
{
    #region Update BlockName
    private readonly UpdateBlockNameCommand _updateBlockNameCommand;
    private readonly UpdateBlockNameCommandHandler _updateBlockNameCommandHandler;
    private readonly UpdateBlockNameCommandValidator _updateBlockNameValidator;
    public UpdateBlockTests(BlockFakeDatas fakeData, UpdateBlockNameCommand updateBlockNameCommand, UpdateBlockNameCommandValidator updateBlockNameValidator) : base(fakeData)
    {
        _updateBlockNameCommand = updateBlockNameCommand;
        _updateBlockNameValidator = updateBlockNameValidator;
        _updateBlockNameCommandHandler = new(MockRepository.Object, Mapper,BusinessRules);
    }
    [Fact]
    public void UpdateBlockNameEmpty_ShouldReturn_ValidationError()
    {
        //Arrange
        _updateBlockNameCommand.Name = string.Empty;

        //Act
        ValidationFailure? result = _updateBlockNameValidator.Validate(_updateBlockNameCommand)
            .Errors.FirstOrDefault();

        //Assert
        Assert.Equal(BlockMessages.ValidationMessages.BlockNameCannotBeEmpty, result?.ErrorMessage);
    }
    [Fact]
    public void UpdateBlockNameStartsWithNumber_ShouldReturn_ValidationError()
    {
        //Arrange
        _updateBlockNameCommand.Name = "1A";

        //Act
        ValidationFailure? result = _updateBlockNameValidator.Validate(_updateBlockNameCommand)
            .Errors.FirstOrDefault();

        //Assert
        Assert.Equal(BlockMessages.ValidationMessages.BlockNameMustStartWithALetter, result?.ErrorMessage);
    }
    [Fact]
    public void UpdateBlockNameLongerThanTwoCharacters_ShouldReturn_ValidationError()
    {
        //Arrange
        _updateBlockNameCommand.Name = "ABC";

        //Act
        ValidationFailure? result = _updateBlockNameValidator.Validate(_updateBlockNameCommand)
            .Errors.FirstOrDefault();

        //Assert
        Assert.Equal(BlockMessages.ValidationMessages.BlockNameCannotBeLongerThanTwoCharacters, result?.ErrorMessage);
    }
    [Fact]
    public async Task WhenUpdateBlockIdDoesNotExistInDb_ShouldReturn_BusinessException()
    {
        //Arrange
        _updateBlockNameCommand.Id = BlockFakeDatas.NotInDbId;
        _updateBlockNameCommand.Name = BlockFakeDatas.InDbBlockName;
        //Act
        Task Action() => _updateBlockNameCommandHandler.Handle(_updateBlockNameCommand, CancellationToken.None);
        //Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(Action);
        Assert.Equal(BlockMessages.RuleMessages.BlocIsNotExist, exception.Message);

    }
    [Fact]
    public async Task UpdateBlockNameExistsInDatabase_ShouldReturn_BusinessException()
    {
        //Arrange
        _updateBlockNameCommand.Id = BlockFakeDatas.InDbId;
        _updateBlockNameCommand.Name = BlockFakeDatas.InDbBlockName;
        //Act
        Task Action() => _updateBlockNameCommandHandler.Handle(_updateBlockNameCommand, CancellationToken.None);
        //Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(Action);
        Assert.Equal(BlockMessages.RuleMessages.BlockNameAlreadyExist, exception.Message);
    }
    [Fact]
    public async Task UpdateBlockNameSuccessfully_Should_CalledUpdateAsyncOnce()
    {
        //Arrange
        _updateBlockNameCommand.Id = BlockFakeDatas.InDbId;
        _updateBlockNameCommand.Name = BlockFakeDatas.NotInDbBlockName;
        //Act
        var response = await _updateBlockNameCommandHandler.Handle(_updateBlockNameCommand, CancellationToken.None);
        //Assert
        MockRepository.Verify(s => s.UpdateAsync(It.IsAny<Block>(), CancellationToken.None), Times.Once());
        Assert.Equal(BlockFakeDatas.NotInDbBlockName, response.NewName );
        Assert.NotNull(response);
        
    }   
    #endregion
}
