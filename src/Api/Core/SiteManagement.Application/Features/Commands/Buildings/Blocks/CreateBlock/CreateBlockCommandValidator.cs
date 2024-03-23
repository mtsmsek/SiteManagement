using FluentValidation;
using SiteManagement.Domain.Constants.Buildings.Blocks;

namespace SiteManagement.Application.Features.Commands.Buildings.Blocks.CreateBlock;

public class CreateBlockCommandValidator : AbstractValidator<CreateBlockCommand>
{
    public CreateBlockCommandValidator()
    {
        RuleFor(block => block.Name).NotEmpty().WithMessage(BlockMessages.ValidationMessages.BlockNameCannotBeEmpty);

    }
}
