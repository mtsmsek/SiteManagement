using MediatR;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Domain.Residency;

namespace SiteManagement.Application.Residency.Queries.GetResidentById;

/// <summary>Returns the resident detail DTO; 404 when no resident has that id.</summary>
public sealed class GetResidentByIdQueryHandler(IResidentQueries residentQueries)
    : IRequestHandler<GetResidentByIdQuery, ResidentDetailsDto>
{
    private readonly IResidentQueries _residentQueries = residentQueries;

    /// <inheritdoc />
    public async Task<ResidentDetailsDto> Handle(GetResidentByIdQuery request, CancellationToken cancellationToken)
        => await _residentQueries.GetByIdAsync(request.ResidentId, cancellationToken)
           ?? throw new EntityNotFoundException(nameof(Resident), request.ResidentId);
}
