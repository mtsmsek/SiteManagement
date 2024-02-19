using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Features.Commands.Buildings.Blocks.UpdateBlock.UpdateBlockName
{
    public class UpdateBlockNameCommand : IRequest<UpdateBlockNameResponse>
    {
        private string _name;
        public Guid Id { get; set; }
        public string Name { get { return _name; } set { _name = value.ToUpper(); } }
    }
}
