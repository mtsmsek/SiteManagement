using AutoMapper;
using MediatR;
using SiteManagement.Application.Rules.Buildings.Blocks;
using SiteManagement.Application.Services.Repositories.Buildings;
using SiteManagement.Domain.Constants.Buildings.Blocks;
using SiteManagement.Domain.Entities.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Features.Queries.Blocks.GetBlockDetailByName
{
    public class GetBlockDetailByNameQueryHandler : IRequestHandler<GetBlockDetailByNameQuery, GetBlockDetailByNameResponse>
    {
        private readonly IBlockRepository _blockRepository;
        private readonly IMapper _mapper;
        private readonly BlockBusinessRules _blockBusinessRules;

        public GetBlockDetailByNameQueryHandler(IBlockRepository blockRepository, IMapper mapper, BlockBusinessRules blockBusinessRules)
        {
            _blockRepository = blockRepository;
            _mapper = mapper;
            _blockBusinessRules = blockBusinessRules;
        }

        public async Task<GetBlockDetailByNameResponse> Handle(GetBlockDetailByNameQuery request, CancellationToken cancellationToken)
        {
            Block block = await _blockBusinessRules.BlockShouldBeExistInDatabase(request.Name);

            return _mapper.Map<GetBlockDetailByNameResponse>(block);

           
        }
    }
}
