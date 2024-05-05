using FluentValidation;
using SiteManagement.Domain.Constants.Buildings.Blocks;

namespace SiteManagement.Application.Features.Commands.Buildings.Blocks.UpdateBlock.UpdateBlockName;

public class UpdateBlockNameCommandValidator : AbstractValidator<UpdateBlockNameCommand>
{
    public UpdateBlockNameCommandValidator()
    {

        RuleFor(block => DeleteSpaces(block.Name))
            .NotEmpty().WithMessage(BlockMessages.ValidationMessages.BlockNameCannotBeEmpty)
            .Must(StartWithLetter).WithMessage(BlockMessages.ValidationMessages.BlockNameMustStartWithALetter)
            .Length(1, 2).WithMessage(BlockMessages.ValidationMessages.BlockNameCannotBeLongerThanTwoCharacters);


    }
    private bool StartWithLetter(string name) => char.IsLetter(name.FirstOrDefault());
    private static string DeleteSpaces(string name) => name.TrimStart().TrimEnd();

}
