using FluentValidation;
using SiteManagement.Application.Features.Commands.Security.OperationClaims.CreateOperationClaim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Features.Commands.Security.OperationClaims.UpdateOperationClaim
{
    public class UpdateOperationClaimValidator : AbstractValidator<UpdateOperationClaimCommand>
    {
        public UpdateOperationClaimValidator()
        {
            
                RuleFor(oc => oc.Name).NotEmpty().WithMessage("Opeartion claim name cannot be empty");

            
        }
    }
}
