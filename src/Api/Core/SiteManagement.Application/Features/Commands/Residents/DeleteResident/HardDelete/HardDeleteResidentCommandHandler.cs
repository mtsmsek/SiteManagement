using MediatR;
using SiteManagement.Application.Rules.Residents;
using SiteManagement.Application.Services.Repositories.Residents;

namespace SiteManagement.Application.Features.Commands.Residents.DeleteResident.HardDelete
{
    public class HardDeleteResidentCommandHandler : IRequestHandler<HardDeleteResidentCommand, Guid>
    {
        private readonly IResidentRepository _residentRepository;
        private readonly ResidentBusinessRules _residentBusinessRules;

        public HardDeleteResidentCommandHandler(IResidentRepository residentRepository, ResidentBusinessRules residentBusinessRules)
        {
            _residentRepository = residentRepository;
            _residentBusinessRules = residentBusinessRules;
        }

        public async Task<Guid> Handle(HardDeleteResidentCommand request, CancellationToken cancellationToken)
        {
            var resident = await _residentBusinessRules.CheckIfResidentExistById(request.Id,cancellationToken);

            await _residentRepository.DeleteAsync(entity: resident,
                                                  isPermenant: true,
                                                  cancellationToken: cancellationToken);

            return request.Id;
        }
    }
}
