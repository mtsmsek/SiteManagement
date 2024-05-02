using FluentValidation;
using SiteManagement.Domain.Constants.Buildings.Blocks;

namespace SiteManagement.Application.Features.Commands.Buildings.Blocks.CreateBlock;

public class CreateBlockCommandValidator : AbstractValidator<CreateBlockCommand>
{
    public CreateBlockCommandValidator()
    {

        RuleFor(block => DeleteSpaces(block.Name))
            .NotEmpty().WithMessage(BlockMessages.ValidationMessages.BlockNameCannotBeEmpty)
            .Must(StartWithLetter).WithMessage(BlockMessages.ValidationMessages.BlockNameMustStartWithALetter)
            .Length(1, 2).WithMessage(BlockMessages.ValidationMessages.BlockNameCannotBeLongerThanTwoCharacters);


    }
    private bool StartWithLetter(string name) =>  char.IsLetter(name.FirstOrDefault());
    private static string DeleteSpaces(string name) => name.TrimStart().TrimEnd();



}
