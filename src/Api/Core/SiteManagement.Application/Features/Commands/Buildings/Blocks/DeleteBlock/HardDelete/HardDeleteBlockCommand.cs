using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Features.Commands.Buildings.Blocks.DeleteBlock.HardDelete;

public class HardDeleteBlockCommand : IRequest<Guid>
{
    public Guid Id { get; set; }

}
