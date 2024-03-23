using FluentValidation;
using SiteManagement.Application.Features.Commands.Buildings.Blocks.CreateBlock;
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
            //TODO -- remove magic strings
            RuleFor(x => x.Email).NotEmpty().When(x => string.IsNullOrEmpty(x.IdenticalNumber)).WithMessage("Lütfen email ya da TC numaranızı giriniz");
            RuleFor(x => x.IdenticalNumber).NotEmpty().When(x => string.IsNullOrEmpty(x.Email)).WithMessage("Lütfen email ya da TC numaranızı giriniz");
        }
    }
}
