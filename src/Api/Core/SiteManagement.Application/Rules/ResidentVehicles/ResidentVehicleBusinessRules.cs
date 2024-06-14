using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;
using SiteManagement.Application.Rules.Commons;
using SiteManagement.Application.Rules.Residents;
using SiteManagement.Application.Services.Repositories.Vehicles;
using SiteManagement.Domain.Constants.Vehicles;
using SiteManagement.Domain.Entities.Vehicles;

namespace SiteManagement.Application.Rules.ResidentVehicles;

public class ResidentVehicleBusinessRules : BaseBusinessRules
{
    private readonly IResidentVehicleRepository _residentVehicleRepository;
    private readonly ResidentBusinessRules _residentBusinessRules;

    public ResidentVehicleBusinessRules(IResidentVehicleRepository residentVehicleRepository, ResidentBusinessRules residentBusinessRules)
    {
        _residentVehicleRepository = residentVehicleRepository;
        _residentBusinessRules = residentBusinessRules;
    }

    public async Task<ResidentVehicle> CheckIfResidentVehicleExistById(Guid id, CancellationToken cancellationToken)
    {

        var dbResidentVehicle = await _residentVehicleRepository.GetByIdAsync(id, cancellationToken: cancellationToken) ??
                                 throw new BusinessException(ResidentVehicleMessages.RuleMessages.ResidentOrVehicleCannotBeFound);

        return dbResidentVehicle;

    }
}
