using MediatR;
using SiteManagement.Application.Features.Queries.Blocks.GetBlockDetailByName;
using SiteManagement.Application.Pagination.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Features.Queries.Blocks.GetBlockDetailByName
{
    public class GetBlockDetailByNameQuery : PageRequest, IRequest<GetBlockDetailByNameResponse>
    {
        private string _name;
        public string Name { get => _name; set => _name = value.ToUpper(); }
    }
}
