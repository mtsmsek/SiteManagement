using FluentValidation;

namespace SiteManagement.Application.Features.Commands.Security.OperationClaims.CreateOperationClaim
{
    public class CreateOperationClaimCommandValidator : AbstractValidator<CreateOperationClaimCommand>
    {
        public CreateOperationClaimCommandValidator()
        {
            RuleFor(oc => oc.Name).NotEmpty().WithMessage("Opeartion claim name cannot be empty");

        }
    }
}
