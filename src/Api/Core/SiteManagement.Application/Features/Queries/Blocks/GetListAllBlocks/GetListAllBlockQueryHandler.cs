using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Features.Queries.Blocks.GetListAllBlocks
{
    public class GetListAllBlockQueryHandler : IRequestHandler<GetListAllBlockQuery, GetListAllBlockResponse>
    {
        public Task<GetListAllBlockResponse> Handle(GetListAllBlockQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
