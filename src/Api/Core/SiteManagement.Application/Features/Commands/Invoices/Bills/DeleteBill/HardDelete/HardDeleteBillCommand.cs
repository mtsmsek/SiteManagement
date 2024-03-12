using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Features.Commands.Invoices.Bills.DeleteBill.HardDelete
{
    public class HardDeleteBillCommand : IRequest<Guid>
    {
        public Guid Id { get; set; }
    }
}
