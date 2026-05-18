using FluentValidation;
using SiteManagement.Application.Shared.Validation;

namespace SiteManagement.Application.Property.Commands.MarkApartmentEmpty;

/// <summary>Surface-level validation for <see cref="MarkApartmentEmptyCommand"/>.</summary>
public sealed class MarkApartmentEmptyCommandValidator : AbstractValidator<MarkApartmentEmptyCommand>
{
    public MarkApartmentEmptyCommandValidator()
    {
        RuleFor(x => x.ApartmentId).ValidId();
    }
}
