using AutoMapper;
using MediatR;
using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;
using SiteManagement.Application.Rules.Buildings.Apartments;
using SiteManagement.Application.Services.Repositories.Buildings;
using SiteManagement.Domain.Entities.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Features.Commands.Buildings.Apartments.UpdateApartment.ChangeResidentStatus
{
    public class ChangeResidentStatusCommandHandler : IRequestHandler<ChangeResidentStatusCommand, bool>
    {
        private readonly IApartmentRepository _apartmentRepository;
        private readonly IMapper _mapper;
        private readonly ApartmentBusinessRules _apartmentBusinessRules;

        public ChangeResidentStatusCommandHandler(IApartmentRepository apartmentRepository, IMapper mapper, ApartmentBusinessRules apartmentBusinessRules)
        {
            _apartmentRepository = apartmentRepository;
            _mapper = mapper;
            _apartmentBusinessRules = apartmentBusinessRules;
        }

        public async Task<bool> Handle(ChangeResidentStatusCommand request, CancellationToken cancellationToken)
        {
            //TODO -- it can be handle with rabbit mq ??
           Apartment apartment = await _apartmentBusinessRules.ApartmentShouldExistInDatabase(request.Id, cancellationToken);

            _mapper.Map(apartment, request);

            await _apartmentRepository.UpdateAsync(apartment, cancellationToken);

            return request.Status;
        }
        
    }
}
