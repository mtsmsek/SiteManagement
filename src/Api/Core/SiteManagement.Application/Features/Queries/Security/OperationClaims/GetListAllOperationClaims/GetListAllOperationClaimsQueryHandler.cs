using AutoMapper;
using MediatR;
using SiteManagement.Application.Pagination.Responses;
using SiteManagement.Application.Services.Repositories.Security;

namespace SiteManagement.Application.Features.Queries.Security.OperationClaims.GetListAllOperationClaims;

public class GetListAllOperationClaimsQueryHandler : IRequestHandler<GetListAllOperationClaimsQuery, PagedViewModel<GetListAllOperationClaimsResponse>>
{
    private readonly IOperationClaimRepository _operationClaimRepository;
    private readonly IMapper _mapper;

    public GetListAllOperationClaimsQueryHandler(IOperationClaimRepository operationClaimRepository, IMapper mapper)
    {
        _operationClaimRepository = operationClaimRepository;
        _mapper = mapper;
    }

    public async Task<PagedViewModel<GetListAllOperationClaimsResponse>> Handle(GetListAllOperationClaimsQuery request, CancellationToken cancellationToken)
    {
        var operationClaims = await _operationClaimRepository.GetListAsync();

        return _mapper.Map<PagedViewModel<GetListAllOperationClaimsResponse>>(operationClaims);
    }
}
