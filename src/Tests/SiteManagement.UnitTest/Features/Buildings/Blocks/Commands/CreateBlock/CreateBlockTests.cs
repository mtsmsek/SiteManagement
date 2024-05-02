using FluentValidation.Results;
using SiteManagement.Application.Features.Commands.Buildings.Blocks.CreateBlock;
using SiteManagement.UnitTest.Mock.FakeDatas.Buildings;
using SiteManagement.UnitTest.Mock.Repositories.Buildings;

namespace SiteManagement.UnitTest.Features.Buildings.Blocks.Commands.CreateBlock;

public class CreateBlockTests : BlockMockRepository
{
    private readonly CreateBlockCommandValidator _validator;
    private readonly CreateBlockCommand _command;
    private readonly CreateBlockCommandHandler _handler;

    public CreateBlockTests(BlockFakeDatas fakeData, CreateBlockCommand command, CreateBlockCommandValidator validator) : base(fakeData)
    {

        _validator = validator;
        _command = command;
        _handler = new CreateBlockCommandHandler(MockRepository.Object, Mapper, BusinessRules);
    }

    [Test]
    public void BlockNameEmptyShouldReturnValidationError()
    {
        _command.Name = string.Empty;
        ValidationFailure? result = _validator.Validate(_command)
            .Errors.Where(x => x.PropertyName == nameof(_command.Name) && x.ErrorCode == "Block name must be filled").FirstOrDefault();

        Assert.Equals("Block name must be filled", result?.ErrorCode);
    }
}
