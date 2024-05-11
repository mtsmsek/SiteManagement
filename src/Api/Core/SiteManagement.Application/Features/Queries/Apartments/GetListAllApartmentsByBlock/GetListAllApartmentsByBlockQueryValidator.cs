using FluentValidation;
using SiteManagement.Domain.Constants.Buildings.Apartments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Features.Queries.Apartments.GetListAllApartmentsByBlock;

public class GetListAllApartmentsByBlockQueryValidator : AbstractValidator<GetListAllApartmentsByBlockQuery>
{
    public GetListAllApartmentsByBlockQueryValidator()
    {
        RuleFor(x => x).Custom((x, context) =>
        {
            if (x.BlockId == Guid.Empty && string.IsNullOrEmpty(x.BlockName))
            {
                context.AddFailure(ApartmentMessages.ValidationMessages.BlockIdAndBlockNameCannotBeEmptyAtTheSameTime);
            }
        });
    }
}
