using MediatR;
using SiteManagement.Domain.Enumarations.Invoices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Features.Commands.Invoices.Bills.CreateBill
{
    public class CreateBillCommand : IRequest<Guid>
    {
        public BillType Type { get; set; }
        public double Fee { get; set; }
    }
}
