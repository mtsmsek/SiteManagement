﻿using MediatR;
using SiteManagement.Application.CrossCuttingConcerns.Caching;

namespace SiteManagement.Application.Features.Commands.Buildings.Apartments.UpdateApartment.ChangeTenantStatus;

public class ChangeTenantStatusCommand : IRequest<bool>, ICacheRemoverRequest
{
    public Guid Id { get; set; }
    public bool IsTenant { get; set; }

    public string? CacheKey => "GetAllApartments";

    public bool BypassCache { get; }
}
