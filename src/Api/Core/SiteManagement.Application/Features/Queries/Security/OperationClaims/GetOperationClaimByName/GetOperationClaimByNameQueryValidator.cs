using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Features.Queries.Security.OperationClaims.GetOperationClaimByName
{
    public class GetOperationClaimByNameQueryValidator : AbstractValidator<GetOperationClaimByNameQuery>
    {
        public GetOperationClaimByNameQueryValidator()
        {
            //todo -- remove magic string
            RuleFor(g => g.Name).NotEmpty().WithMessage("Operation claim adı boş geçilemez");
        }
    }
}
