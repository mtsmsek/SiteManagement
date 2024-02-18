using AutoMapper;
using MediatR;
using SiteManagement.Application.Pagination.Responses;
using SiteManagement.Application.Services.Repositories.Buildings;
using SiteManagement.Domain.Entities.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Features.Queries.Blocks.GetListAllBlocks
{
    public class GetListAllBlockQueryHandler : IRequestHandler<GetListAllBlockQuery, PagedViewModel<GetListAllBlockResponse>>
    {
        private readonly IBlockRepository _blockRepository;
        private readonly IMapper _mapper;

        public GetListAllBlockQueryHandler(IBlockRepository blockRepository, IMapper mapper)
        {
            _blockRepository = blockRepository;
            _mapper = mapper;
        }

        public async Task<PagedViewModel<GetListAllBlockResponse>> Handle(GetListAllBlockQuery request, CancellationToken cancellationToken)
        {
            
            PagedViewModel<Block> blocks = await _blockRepository.GetListAsync(currentPage: request.Page,
                                                                               pageSize: request.PageSize,
                                                                               cancellationToken: cancellationToken);
          PagedViewModel<GetListAllBlockResponse> response = _mapper.Map<PagedViewModel<GetListAllBlockResponse>>(blocks);

            return response;
        }
    }
}
