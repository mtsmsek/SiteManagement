using AutoMapper;
using MediatR;
using SiteManagement.Application.Rules.Residents;
using SiteManagement.Application.Services.Repositories.Residents;
using SiteManagement.Application.Services.Repositories.Security;
using SiteManagement.Domain.Entities.Residents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Features.Commands.Residents.UpdateResident.UpdateEmail
{
    public class UpdateResidentEmailCommandHandler : IRequestHandler<UpdateResidentEmailCommand, UpdateResidentEmailResponse>
    {
        private readonly IResidentRepository _residentRepository;
        private readonly ResidentBusinessRules _residentBusinessRules;
        public UpdateResidentEmailCommandHandler(IResidentRepository residentRepository, ResidentBusinessRules residentBusinessRules)
        {
            _residentRepository = residentRepository;

            _residentBusinessRules = residentBusinessRules;
        }

        public async Task<UpdateResidentEmailResponse> Handle(UpdateResidentEmailCommand request, CancellationToken cancellationToken)
        {
            Resident resident = await _residentBusinessRules.CheckIfResidentExistById(request.Id, cancellationToken);
            
            _residentBusinessRules.EmailCannotBeSameWithOldEmail(resident.Email,request.Email);

            //TODO -- send confirm link for email with rabbitmq
            resident.Email = request.Email;

            await _residentRepository.UpdateAsync(resident);

            return new UpdateResidentEmailResponse 
            {
                Email = resident.Email
            };



        }
    }
}
