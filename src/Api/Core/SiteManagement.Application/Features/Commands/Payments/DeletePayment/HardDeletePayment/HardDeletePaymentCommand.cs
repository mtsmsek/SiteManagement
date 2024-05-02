using MediatR;

namespace SiteManagement.Application.Features.Commands.Payments.DeletePayment.HardDeletePayment
{
    public class HardDeletePaymentCommand : IRequest
    {
        public Guid Id { get; set; }
    }
}
