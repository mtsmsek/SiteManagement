using FluentValidation;
using SiteManagement.Application.Features.Commands.Buildings.Blocks.CreateBlock;
using SiteManagement.Domain.Constants.Residents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Features.Commands.Residents.Login
{
    public class ResidentLoginCommandValidator : AbstractValidator<ResidentLoginCommand>
    {
        public ResidentLoginCommandValidator()
        {
            RuleFor(x => x.Email).NotEmpty().When(x => string.IsNullOrEmpty(x.IdenticalNumber))
            .WithMessage(ResidentMessages.ValidationMessages.EmailOrIdenticalNumberCannotBeEmpty)
            .EmailAddress(FluentValidation.Validators.EmailValidationMode.AspNetCoreCompatible)
            .When(x => !string.IsNullOrEmpty(x.Email))
            .WithMessage(ResidentMessages.ValidationMessages.InvalidEmail);


            RuleFor(x => x.IdenticalNumber).NotEmpty().When(x => string.IsNullOrEmpty(x.Email))
                .WithMessage(ResidentMessages.ValidationMessages.EmailOrIdenticalNumberCannotBeEmpty)
                .DependentRules
                (() =>
                 RuleFor(x => x.IdenticalNumber)
                .Length(11).When(x => !string.IsNullOrEmpty(x.IdenticalNumber))
                .WithMessage(ResidentMessages.ValidationMessages.IdenticalNumberMustIncludeElevenChar)
                
                );
        }
    }
}
