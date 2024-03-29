using AutoMapper;
using MediatR;
using SiteManagement.Application.Rules.Buildings.Apartments;
using SiteManagement.Application.Services.Repositories.Buildings;
using SiteManagement.Domain.Entities.Buildings;

namespace SiteManagement.Application.Features.Commands.Buildings.Apartments.UpdateApartment.ChangeTenantStatus;

//TODO -- make service for change tenant and residant status when a resident add to an apartment.
public class ChangeTenantStatusCommandHandler : IRequestHandler<ChangeTenantStatusCommand, bool>
{
    private readonly IApartmentRepository _apartmentRepository;
    private readonly IMapper _mapper;
    private readonly ApartmentBusinessRules _apartmentBusinessRules;

    public ChangeTenantStatusCommandHandler(IApartmentRepository apartmentRepository, IMapper mapper, ApartmentBusinessRules apartmentBusinessRules)
    {
        _apartmentRepository = apartmentRepository;
        _mapper = mapper;
        _apartmentBusinessRules = apartmentBusinessRules;
    }

    public async Task<bool> Handle(ChangeTenantStatusCommand request, CancellationToken cancellationToken)
    {
        Apartment apartment =  await _apartmentBusinessRules.ApartmentShouldExistInDatabase(request.Id, cancellationToken);

        _mapper.Map(request, apartment);

        await _apartmentRepository.UpdateAsync(apartment, cancellationToken);

        return request.IsTenant;
    }
}
