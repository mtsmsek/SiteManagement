using AutoMapper;
using MediatR;
using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;
using SiteManagement.Application.Services.Repositories.Security;
using SiteManagement.Domain.Entities.Security;

namespace SiteManagement.Application.Features.Commands.Security.UserOperationClaims.UpdateUserOperationClaim;

public class UpdateUserOperationClaimCommandHandler : IRequestHandler<UpdateUserOperationClaimCommand, UpdateUserOperationClaimResponse>
{
    private readonly IUserOperationClaimRepository _userOperationClaimRepository;
    private readonly IMapper _mapper;
    private readonly IOperationClaimRepository _operationClaimRepository;
    private readonly IUserRepository _userRepository;

    public UpdateUserOperationClaimCommandHandler(IUserOperationClaimRepository userOperationClaimRepository, IMapper mapper, IOperationClaimRepository operationClaimRepository, IUserRepository userRepository)
    {
        _userOperationClaimRepository = userOperationClaimRepository;
        _mapper = mapper;
        _operationClaimRepository = operationClaimRepository;
        _userRepository = userRepository;
    }

    public async Task<UpdateUserOperationClaimResponse> Handle(UpdateUserOperationClaimCommand request, CancellationToken cancellationToken)
    {
        var dbUser = await _userRepository.GetByIdAsync(id: request.UserId, cancellationToken: cancellationToken);

        //todo -- remove magic strings
        if (dbUser == null)
            throw new BusinessException("User cannot be found");

        var dbOperationClaim = await _operationClaimRepository.GetByIdAsync(id: request.OperationClaimId, cancellationToken: cancellationToken);
        if (dbOperationClaim == null)
            throw new BusinessException("Operation claim cannot be found");

        var userOperationClaimToUpdate = _mapper.Map<UserOperationClaim>(request);

        _mapper.Map(request, userOperationClaimToUpdate);

        await _userOperationClaimRepository.UpdateAsync(entity: userOperationClaimToUpdate, cancellationToken: cancellationToken);

        return _mapper.Map<UpdateUserOperationClaimResponse>(userOperationClaimToUpdate);
    }
}
