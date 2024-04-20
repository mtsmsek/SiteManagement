using MediatR;
using SiteManagement.Domain.Entities.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Features.Commands.Payments.CreatePayment
{
    public class CreatePaymentCommand : IRequest<CreatePaymentResponse>
    {
        public Guid UserId { get; set; }
        public Guid BillId { get; set; }
        public CreditCard CreditCard { get; set; }  
    }
}
