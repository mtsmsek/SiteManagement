using FluentValidation;
using SiteManagement.Application.Shared.Validation;

namespace SiteManagement.Application.Tenancy.Commands.EndAssignment;

/// <summary>Surface-level validation for <see cref="EndAssignmentCommand"/>.</summary>
public sealed class EndAssignmentCommandValidator : AbstractValidator<EndAssignmentCommand>
{
    public EndAssignmentCommandValidator()
    {
        RuleFor(x => x.AssignmentId).ValidId();
    }
}
