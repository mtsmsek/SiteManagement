using AutoMapper;
using MediatR;
using SiteManagement.Application.Rules.Residents;
using SiteManagement.Application.Security.Hashing;
using SiteManagement.Application.Services.Repositories.Residents;
using SiteManagement.Application.Services.Repositories.Security;
using SiteManagement.Application.Services.Security;
using SiteManagement.Domain.Constants.Security;
using SiteManagement.Domain.Entities.Residents;
using SiteManagement.Domain.Entities.Security;

namespace SiteManagement.Application.Features.Commands.Residents.CreateResident
{
    public class CreateResidentCommandHandler : IRequestHandler<CreateResidentCommand, CreateResidentResponse>
    {
        const string passwordChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789"; // Characters to choose from
        const int passwordLength = 20;
        private readonly IResidentRepository _residentRepository;
        private readonly ResidentBusinessRules _residentBusinessRules;
        private readonly IMapper _mapper;
        private readonly IUserOperationClaimService _userOperationClaimService;


        public CreateResidentCommandHandler(IResidentRepository residentRepository, ResidentBusinessRules residentBusinessRules, IMapper mapper, IUserOperationClaimService userOperationClaimService)
        {
            _residentRepository = residentRepository;
            _residentBusinessRules = residentBusinessRules;
            _mapper = mapper;
            _userOperationClaimService = userOperationClaimService;
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

            await _userOperationClaimService.AddUserWithOperationClaim(residentToAdd.Id, UsersOperationClaims.User);
            //todo - make this class transactional

            var response = _mapper.Map<CreateResidentResponse>(residentToAdd);
            response.Password = firstPassword;

            return response;
        }

        private string generateRandomPassword(int length)
        {
            Random random = new();
            return new string(Enumerable.Repeat(passwordChars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
