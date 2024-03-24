using AutoMapper;
using MediatR;
using SiteManagement.Application.Rules.Residents;
using SiteManagement.Application.Services.Repositories.Residents;
using SiteManagement.Domain.Entities.Residents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Features.Commands.Residents.UpdateResident.UpdateInformation;

public class UpdateResidentCommandHandler : IRequestHandler<UpdateResidentCommand, UpdateResidentResponse>
{
    private readonly IResidentRepository _residentRepository;
    private readonly IMapper _mapper;
    private readonly ResidentBusinessRules _residentBusinessRules;

    public UpdateResidentCommandHandler(IResidentRepository residentRepository, IMapper mapper, ResidentBusinessRules residentBusinessRules)
    {
        _residentRepository = residentRepository;
        _mapper = mapper;
        _residentBusinessRules = residentBusinessRules;
    }

    public async Task<UpdateResidentResponse> Handle(UpdateResidentCommand request, CancellationToken cancellationToken)
    {
        Resident residentToUpdate = await _residentBusinessRules.CheckIfResidentExistById(request.Id, cancellationToken);

        residentToUpdate = _mapper.Map(request, residentToUpdate);

        await _residentRepository.AddAsync(residentToUpdate, cancellationToken);
        return _mapper.Map<UpdateResidentResponse>(residentToUpdate);

    }
}
