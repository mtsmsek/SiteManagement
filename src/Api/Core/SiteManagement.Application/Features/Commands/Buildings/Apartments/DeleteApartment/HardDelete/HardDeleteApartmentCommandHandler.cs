using AutoMapper;
using MediatR;
using SiteManagement.Application.Rules.Buildings.Apartments;
using SiteManagement.Application.Services.Repositories.Buildings;
using SiteManagement.Domain.Entities.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Features.Commands.Buildings.Apartments.DeleteApartment.HardDelete
{
    public class HardDeleteApartmentCommandHandler : IRequestHandler<HardDeleteApartmentCommand, Guid>
    {
        private readonly IApartmentRepository _apartmentRepository;
        private readonly IMapper _mapper;
        private readonly ApartmentBusinessRules _apartmentBusinessRules;

        public HardDeleteApartmentCommandHandler(IApartmentRepository repository, IMapper mapper, ApartmentBusinessRules apartmentBusinessRules)
        {
            _apartmentRepository = repository;
            _mapper = mapper;
            _apartmentBusinessRules = apartmentBusinessRules;
        }

        public async Task<Guid> Handle(HardDeleteApartmentCommand request, CancellationToken cancellationToken)
        {
            Apartment apartment = await _apartmentBusinessRules.ApartmentShouldExistInDatabase(request.Id);

            _mapper.Map(apartment, request);

            await _apartmentRepository.DeleteAsync(entity: apartment,
                                             isPermenant: true,
                                             cancellationToken: cancellationToken);
            return request.Id;

            
        }
    }
}
