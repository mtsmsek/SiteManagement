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

namespace SiteManagement.Application.Features.Commands.Buildings.Apartments.CreateApartment
{
    public class CreateApartmentCommandHandler : IRequestHandler<CreateApartmentCommand, CreateApartmentResponse>
    {
        private readonly IApartmentRepository _apartmentRepository;
        private readonly IMapper _mapper;
        private readonly ApartmentBusinessRules _apartmentBusinessRules;

        public CreateApartmentCommandHandler(IApartmentRepository apartmentRepository, IMapper mapper, ApartmentBusinessRules apartmentBusinessRules)
        {
            _apartmentRepository = apartmentRepository;
            _mapper = mapper;
            _apartmentBusinessRules = apartmentBusinessRules;
        }

        public async Task<CreateApartmentResponse> Handle(CreateApartmentCommand request, CancellationToken cancellationToken)
        {
            await _apartmentBusinessRules.ApartmentNumberCannotBeDuplicateForSameBlock(request.BlockId, request.ApartmentNumber);
            var apartmentToAdd = _mapper.Map<Apartment>(request);

            await _apartmentRepository.AddAsync(apartmentToAdd);

            var response = _mapper.Map<CreateApartmentResponse>(apartmentToAdd);
            return response;
        }
    }
}
