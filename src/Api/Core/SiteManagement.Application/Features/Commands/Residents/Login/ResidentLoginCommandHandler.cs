using MediatR;
using SiteManagement.Application.Rules.Residents;
using SiteManagement.Application.Security.JWT;
using SiteManagement.Application.Services.Repositories.Security;
using SiteManagement.Domain.Entities.Residents;
using SiteManagement.Domain.Entities.Security;

namespace SiteManagement.Application.Features.Commands.Residents.Login;

public class ResidentLoginCommandHandler : IRequestHandler<ResidentLoginCommand, ResidentLoginCommandResponse>
{
    private readonly ResidentBusinessRules _residentBusinessRules;
    private readonly ITokenHelper _tokenHelper;
    private readonly IUserOperationClaimRepository _userOperationClaimRepository;


    public ResidentLoginCommandHandler(ResidentBusinessRules residentBusinessRules, ITokenHelper tokenHelper, IUserOperationClaimRepository userOperationClaimRepository)
    {
        _residentBusinessRules = residentBusinessRules;
        _tokenHelper = tokenHelper;
        _userOperationClaimRepository = userOperationClaimRepository;
    }

    public async Task<ResidentLoginCommandResponse> Handle(ResidentLoginCommand request, CancellationToken cancellationToken)
    {
        Resident resident; 
        if (!string.IsNullOrEmpty(request.Email))
            resident =  await _residentBusinessRules.CheckIfResidentExistByEmailWhenLogin(email: request.Email!,
                                                                cancellationToken: cancellationToken);

        else
            resident = await _residentBusinessRules.CheckIfResidentExistByIdenticalNumberWhenLogin(identicalNumber: request.IdenticalNumber!,
                                                                                cancellationToken: cancellationToken);
            

        _residentBusinessRules.CheckIfPasswordIsTrue(request.Password, resident.PasswordHash, resident.PasswordSalt);

       var userOperationClaims =  await _userOperationClaimRepository.GetListAsync(predicate: uoc => uoc.UserId == resident.Id, 
                                                                                    includes: uoc => uoc.OperationClaim,
                                                                                   cancellationToken: cancellationToken);

      
        var token = _tokenHelper.CreateToken(resident, userOperationClaims.Results.Select(u => new OperationClaim
        {
            Id = u.Id,
            Name = u.OperationClaim.Name,

        }).ToList());

        
        return new ResidentLoginCommandResponse()
        {
            Id = resident.Id,
            FirstName = resident.FirstName,
            LastName = resident.LastName,
            AccessToken = token
        };
    }
    



}
