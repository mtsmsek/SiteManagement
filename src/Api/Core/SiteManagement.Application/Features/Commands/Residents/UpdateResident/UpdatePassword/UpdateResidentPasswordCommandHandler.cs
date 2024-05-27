using MediatR;
using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;
using SiteManagement.Application.Rules.Residents;
using SiteManagement.Application.Security.Hashing;
using SiteManagement.Application.Services.Repositories.Residents;
using SiteManagement.Domain.Constants.Residents;
using SiteManagement.Domain.Entities.Residents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Features.Commands.Residents.UpdateResident.UpdatePassword
{
    public class UpdateResidentPasswordCommandHandler : IRequestHandler<UpdateResidentPasswordCommand>
    {
        private readonly IResidentRepository _residentRepository;
        private readonly ResidentBusinessRules _residentBusinessRules;
        

        public UpdateResidentPasswordCommandHandler(IResidentRepository residentRepository, ResidentBusinessRules residentBusinessRules)
        {
            _residentRepository = residentRepository;
            _residentBusinessRules = residentBusinessRules;
        }

        public async Task Handle(UpdateResidentPasswordCommand request, CancellationToken cancellationToken)
        {
            //TODO-- add forgot password
           Resident resident = await _residentBusinessRules.CheckIfResidentExistById(request.Id, cancellationToken);

            var isOldPasswordTrue =  HashingHelper.VerifyPasswordHash(request.OldPassword,resident.PasswordHash, resident.PasswordSalt);

            if (!isOldPasswordTrue)
                throw new BusinessException(ResidentMessages.RuleMessages.OldPasswordWrong);

            HashingHelper.CreatePasswordHash(request.NewPassword, out byte[] newPasswordHash, out byte[] newPasswordSalt);

            resident.PasswordHash = newPasswordHash;
            resident.PasswordSalt = newPasswordSalt;

            await _residentRepository.UpdateAsync(resident,cancellationToken);
        }

    }
}
