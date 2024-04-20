using AutoMapper;
using MediatR;
using SiteManagement.Application.Rules.Invoices.Bills;
using SiteManagement.Application.Rules.Payments;
using SiteManagement.Application.Services.Repositories.Payments;
using SiteManagement.Domain.Entities.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Features.Commands.Payments.CreatePayment
{
    public class CreatePaymentCommandHandler : IRequestHandler<CreatePaymentCommand, CreatePaymentResponse>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IMapper _mapper;
        private readonly PaymentBusinessRules _paymentBusinessRules;
        private readonly BillBusinessRules _billBusinessRules;

        public async Task<CreatePaymentResponse> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
        {
            var billToBePaid = await _billBusinessRules.BillShouldBeExistInDatabase(billId: request.BillId);


            await _paymentBusinessRules.AreCardInformationAndBalanceValid(request.CreditCard, billToBePaid.Fee);

            var payment = _mapper.Map<Payment>(request);


            await _paymentRepository.AddAsync(payment);


            return new CreatePaymentResponse();

        }
    }
}
