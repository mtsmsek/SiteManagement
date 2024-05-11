using AutoMapper;
using MediatR;
using SiteManagement.Application.Rules.Buildings.Apartments;
using SiteManagement.Application.Rules.Buildings.Blocks;
using SiteManagement.Application.Services.Repositories.Buildings;
using SiteManagement.Domain.Entities.Buildings;

namespace SiteManagement.Application.Features.Commands.Buildings.Apartments.CreateApartment;


public class CreateApartmentCommandHandler : IRequestHandler<CreateApartmentCommand, CreateApartmentResponse>
{
    private readonly IApartmentRepository _apartmentRepository;
    private readonly IMapper _mapper;
    private readonly ApartmentBusinessRules _apartmentBusinessRules;
    private readonly BlockBusinessRules _blokcBusinessRules;

    public CreateApartmentCommandHandler(IApartmentRepository apartmentRepository, IMapper mapper, ApartmentBusinessRules apartmentBusinessRules, BlockBusinessRules blokcBusinessRules)
    {
        _apartmentRepository = apartmentRepository;
        _mapper = mapper;
        _apartmentBusinessRules = apartmentBusinessRules;
        _blokcBusinessRules = blokcBusinessRules;
    }

    public async Task<CreateApartmentResponse> Handle(CreateApartmentCommand request, CancellationToken cancellationToken)
    {

        var block = await _blokcBusinessRules.BlockShouldBeExistInDatabase(request.BlockId);
        await _apartmentBusinessRules.ApartmentNumberCannotBeDuplicateForSameBlock(request.BlockId, request.ApartmentNumber);
        var apartmentToAdd = _mapper.Map<Apartment>(request);

        await _apartmentRepository.AddAsync(apartmentToAdd);

        var response = _mapper.Map<CreateApartmentResponse>(apartmentToAdd);
        response.BlockName = block.Name;
        return response;
    }
}
