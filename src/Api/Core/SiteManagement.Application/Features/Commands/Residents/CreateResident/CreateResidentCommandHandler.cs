using AutoMapper;
using MediatR;
using SiteManagement.Application.Rules.Residents;
using SiteManagement.Application.Security.Hashing;
using SiteManagement.Application.Services.Repositories.Residents;
using SiteManagement.Domain.Entities.Residents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Features.Commands.Residents.CreateResident
{
    public class CreateResidentCommandHandler : IRequestHandler<CreateResidentCommand, CreateResidentResponse>
    {
        const string passwordChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789"; // Characters to choose from
        const int passwordLength = 20;
        private readonly IResidentRepository _residentRepository;
        private readonly ResidentBusinessRules _residentBusinessRules;
        private readonly IMapper _mapper;

        public CreateResidentCommandHandler(IResidentRepository residentRepository, ResidentBusinessRules residentBusinessRules, IMapper mapper)
        {
            _residentRepository = residentRepository;
            _residentBusinessRules = residentBusinessRules;
            _mapper = mapper;
        }

        public async Task<CreateResidentResponse> Handle(CreateResidentCommand request, CancellationToken cancellationToken)
        {
            await _residentBusinessRules.CheckIfResidentExistByIdenticalNumberWhenInsert(request.IdenticalNumber, cancellationToken);

            var residentToAdd = _mapper.Map<Resident>(request);
            var firstPassword = generateRandomPassword(passwordLength);
            
            HashingHelper.CreatePasswordHash(firstPassword, out byte[] passwordHash, out byte[] passwordSalt);
            residentToAdd.PasswordHash = passwordHash;
            residentToAdd.PasswordSalt = passwordSalt;

            await _residentRepository.AddAsync(residentToAdd);
            //TODO - add validation for adding resident command


            //TODO - investigate how to do it in automapper
            return new CreateResidentResponse
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Password = firstPassword
            };
           //return _mapper.Map<CreateResidentResponse>(residentToAdd);
        }
        private string generateRandomPassword(int length)
        {
            Random random = new Random();
            return new string(Enumerable.Repeat(passwordChars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
