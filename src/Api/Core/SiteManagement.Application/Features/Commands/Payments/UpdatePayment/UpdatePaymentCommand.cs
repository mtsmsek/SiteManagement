using MediatR;

namespace SiteManagement.Application.Features.Commands.Payments.UpdatePayment
{
    public class UpdatePaymentCommand : IRequest<UpdatePaymentResponse>
    {
        public Guid Id { get; set; }
        public Guid ResidentId { get; set; }
        public Guid BillId { get; set; }
    }
}
