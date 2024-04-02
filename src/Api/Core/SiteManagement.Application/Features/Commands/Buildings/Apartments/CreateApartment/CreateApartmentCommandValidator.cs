using FluentValidation;
using SiteManagement.Application.Features.Commands.Buildings.Blocks.CreateBlock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Features.Commands.Buildings.Apartments.CreateApartment
{
    public class CreateApartmentCommandValidator : AbstractValidator<CreateApartmentCommand>
    {
        public CreateApartmentCommandValidator()
        {
            
            RuleFor(x => x.ApartmentNumber).GreaterThanOrEqualTo(1).WithMessage("Apartman numarası 1 den başlamalıdır.");
        }


    }
}
