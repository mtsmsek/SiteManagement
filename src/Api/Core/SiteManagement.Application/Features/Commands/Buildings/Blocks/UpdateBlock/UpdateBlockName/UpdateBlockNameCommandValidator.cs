using FluentValidation;
using SiteManagement.Domain.Constants.Buildings.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Features.Commands.Buildings.Blocks.UpdateBlock.UpdateBlockName
{
    public class UpdateBlockNameCommandValidator : AbstractValidator<UpdateBlockNameCommand>
    {
        public UpdateBlockNameCommandValidator()
        {

            RuleFor(block => DeleteSpaces(block.Name))
                .NotEmpty().WithMessage(BlockMessages.ValidationMessages.BlockNameCannotBeEmpty)
                .Must(StartWithLetter).WithMessage("Block ismi bir harf ile başlamalıdır")
                .Length(1, 2).WithMessage("Block ismi en fazla iki karakterden oluşabilir");


        }
        private bool StartWithLetter(string name) => char.IsLetter(name.FirstOrDefault());
        private static string DeleteSpaces(string name) => name.TrimStart().TrimEnd();

    }
}
