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

        // var token =  _tokenHelper.CreateToken(resident, userOperationClaims.Results.Select(u => u.OperationClaim).ToList());
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
    //"eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTUxMiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjRkN2U2ZWVjLTYxMDktNDUxMy0zZTM1LTA4ZGM0ODZhNTcxNSIsImVtYWlsIjoiZW1pdkBnbWFpbC5jb20iLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiZW1pdiBkdW1hbiIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IkFkbWluIiwibmJmIjoxNzExMDM3MTcxLCJleHAiOjE3MTYyMjExNzEsImlzcyI6InlvdXJfaXNzdWVyX3ZhbHVlIn0.kGUN0VR7qkFGO2OwTJ-uZ7joUmvgxF2EW7ztzcqtFGSRxw237hvMKdLv6RPlZ2r1Yb-ZFIvPO0CF61g5jN1iXA"



}
