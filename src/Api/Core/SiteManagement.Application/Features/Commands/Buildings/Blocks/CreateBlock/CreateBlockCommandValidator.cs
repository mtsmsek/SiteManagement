using FluentValidation;
using SiteManagement.Domain.Constants.Buildings.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Features.Commands.Buildings.Blocks.CreateBlock
{
    public class CreateBlockCommandValidator : AbstractValidator<CreateBlockCommand>
    {
        public CreateBlockCommandValidator()
        {
            RuleFor(block => block.Name).NotEmpty().WithMessage(BlockMessages.ValidationMessages.BlockNameCannotBeEmpty);
        }
    }
}
