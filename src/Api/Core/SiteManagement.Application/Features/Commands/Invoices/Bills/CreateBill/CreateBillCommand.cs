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
        public Guid ApartmentId { get; set; }
        public int Type { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }   
        public double Fee { get; set; }
    }
}
