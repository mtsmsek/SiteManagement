using AutoMapper;
using MediatR;
using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;
using SiteManagement.Application.Services.Repositories.Security;
using SiteManagement.Domain.Entities.Security;

namespace SiteManagement.Application.Features.Commands.Security.UserOperationClaims.CreateUserOperationClaim;

public class CreateUserOperationClaimCommandHandler : IRequestHandler<CreateUserOperationClaimCommand, CreateUserOperationClaimResponse>
{
    private readonly IUserOperationClaimRepository _userOperationClaimRepository;
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;
    private readonly IOperationClaimRepository _operationClaimRepository;

    public CreateUserOperationClaimCommandHandler(IUserOperationClaimRepository userOperationClaimRepository, IMapper mapper, IUserRepository userRepository, IOperationClaimRepository operationClaimRepository)
    {
        _userOperationClaimRepository = userOperationClaimRepository;
        _mapper = mapper;
        _userRepository = userRepository;
        _operationClaimRepository = operationClaimRepository;
    }

    public async Task<CreateUserOperationClaimResponse> Handle(CreateUserOperationClaimCommand request, CancellationToken cancellationToken)
    {
        var dbUser = await _userRepository.GetSingleAsync(predicate: user => user.Id == request.UserId,
                                                          cancellationToken: cancellationToken);

        //todo -- remove magic string
        //todo -- make ISecured and arrange the operation claims
        if (dbUser == null)
            throw new BusinessException("Aradığınız kullanıcı bulunamadı");
        var dbOperationClaim = await _operationClaimRepository.GetSingleAsync(predicate: oc => oc.Id == request.OperationClaimId,
                                                                              cancellationToken: cancellationToken);

        if (dbOperationClaim == null)
            throw new BusinessException("Aradığınız operation claim bulunamadı");

        var userOperationClaimToAdd = _mapper.Map<UserOperationClaim>(request);

        return _mapper.Map<CreateUserOperationClaimResponse>(userOperationClaimToAdd);
    }
}
