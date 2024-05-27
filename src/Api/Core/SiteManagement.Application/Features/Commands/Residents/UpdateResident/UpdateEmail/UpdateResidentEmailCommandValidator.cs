using FluentValidation;
using SiteManagement.Domain.Constants.Residents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Features.Commands.Residents.UpdateResident.UpdateEmail
{
    public class UpdateResidentEmailCommandValidator : AbstractValidator<UpdateResidentEmailCommand>
    {
        public UpdateResidentEmailCommandValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage(ResidentMessages.ValidationMessages.EmailCannotBeEmpty)
                                            .EmailAddress(FluentValidation.Validators.EmailValidationMode.AspNetCoreCompatible)
                                            .WithMessage(ResidentMessages.ValidationMessages.InvalidEmail);
        }
    }
}
