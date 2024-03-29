using MediatR;
using SiteManagement.Application.CrossCuttingConcerns.Caching;

namespace SiteManagement.Application.Features.Commands.Buildings.Apartments.UpdateApartment.ChangeResidentStatus;

public class ChangeResidentStatusCommand : IRequest<bool>, ICacheRemoverRequest
{
    public Guid Id { get; set; }
    public bool Status { get; set; }

    public string? CacheKey => "GetAllApartments";

    public bool BypassCache { get; }
}
