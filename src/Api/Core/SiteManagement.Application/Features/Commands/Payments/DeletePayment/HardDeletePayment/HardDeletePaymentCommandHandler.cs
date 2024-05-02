using MediatR;
using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;
using SiteManagement.Application.Services.Repositories.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Features.Commands.Payments.DeletePayment.HardDeletePayment
{
    public class HardDeletePaymentCommandHandler : IRequestHandler<HardDeletePaymentCommand>
    {
        private readonly IPaymentRepository _paymentRepository;

        public HardDeletePaymentCommandHandler(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task Handle(HardDeletePaymentCommand request, CancellationToken cancellationToken)
        {
           var paymentToDelete =  await _paymentRepository.GetByIdAsync(request.Id);
            
            if (paymentToDelete is null)
                throw new BusinessException("Silmeye çalıştığınız ödeme bulunamadı");
            
            await _paymentRepository.DeleteAsync(paymentToDelete);
            
        }
    }
}
