using MediatR;
using SiteManagement.Application.Rules.Buildings.Apartments;
using SiteManagement.Application.Services.Repositories.Buildings;
using SiteManagement.Domain.Entities.Buildings;

namespace SiteManagement.Application.Features.Commands.Buildings.Apartments.DeleteApartment.HardDelete;

public class HardDeleteApartmentCommandHandler : IRequestHandler<HardDeleteApartmentCommand, Guid>
{
    private readonly IApartmentRepository _apartmentRepository;

    private readonly ApartmentBusinessRules _apartmentBusinessRules;

    public HardDeleteApartmentCommandHandler(IApartmentRepository repository, ApartmentBusinessRules apartmentBusinessRules)
    {
        _apartmentRepository = repository;
        _apartmentBusinessRules = apartmentBusinessRules;
    }

    public async Task<Guid> Handle(HardDeleteApartmentCommand request, CancellationToken cancellationToken)
    {
        Apartment apartment = await _apartmentBusinessRules.ApartmentShouldExistInDatabase(request.Id);

        await _apartmentRepository.DeleteAsync(entity: apartment,
                                         isPermenant: true,
                                         cancellationToken: cancellationToken);
        return request.Id;

        
    }
}
