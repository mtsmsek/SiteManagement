using FluentValidation;
using SiteManagement.Application.Shared.Validation;

namespace SiteManagement.Application.Property.Commands.MarkApartmentOccupied;

/// <summary>Surface-level validation for <see cref="MarkApartmentOccupiedCommand"/>.</summary>
public sealed class MarkApartmentOccupiedCommandValidator : AbstractValidator<MarkApartmentOccupiedCommand>
{
    public MarkApartmentOccupiedCommandValidator()
    {
        RuleFor(x => x.ApartmentId).ValidId();
    }
}
