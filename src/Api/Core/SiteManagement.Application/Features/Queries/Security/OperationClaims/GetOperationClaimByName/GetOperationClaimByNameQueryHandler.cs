using AutoMapper;
using MediatR;
using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;
using SiteManagement.Application.Services.Repositories.Security;

namespace SiteManagement.Application.Features.Queries.Security.OperationClaims.GetOperationClaimByName
{
    public class GetOperationClaimByNameQueryHandler : IRequestHandler<GetOperationClaimByNameQuery, GetOperationClaimByNameResponse>
    {
        private readonly IOperationClaimRepository _operationClaimRepository;
        private readonly IMapper _mapper;

        public GetOperationClaimByNameQueryHandler(IOperationClaimRepository operationClaimRepository, IMapper mapper)
        {
            _operationClaimRepository = operationClaimRepository;
            _mapper = mapper;
        }

        public async Task<GetOperationClaimByNameResponse> Handle(GetOperationClaimByNameQuery request, CancellationToken cancellationToken)
        {
           var operationClaim =  await _operationClaimRepository.GetSingleAsync(predicate: oc => oc.Name == request.Name);

            //rodo -- remove magic string
            if (operationClaim is null)
                throw new BusinessException("Aradığınız isimde bir opeeration claim bulunamadı");

            return _mapper.Map<GetOperationClaimByNameResponse>(operationClaim);
        }
    }
}
