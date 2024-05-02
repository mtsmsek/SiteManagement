using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;
using SiteManagement.Application.Pipelines.Authorization;
using SiteManagement.Application.Pipelines.Transaction;
using SiteManagement.Application.Rules.Invoices.Bills;
using SiteManagement.Application.Rules.Payments;
using SiteManagement.Application.Security.Extensions;
using SiteManagement.Application.Services.Repositories.Invoices;
using SiteManagement.Application.Services.Repositories.Payments;
using SiteManagement.Application.Services.Repositories.Residents;
using SiteManagement.Domain.Entities.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Features.Commands.Payments.CreatePayment
{
    public class CreatePaymentCommandHandler : IRequestHandler<CreatePaymentCommand, CreatePaymentResponse>, ISecuredRequest, ITransactionalRequest
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly PaymentBusinessRules _paymentBusinessRules;
        private readonly BillBusinessRules _billBusinessRules;
        private readonly IBillReposiotry _billRepository;
        private readonly ICreditCardRepository _creditCardRepository;
        private readonly IResidentRepository _residentRepository;

        public CreatePaymentCommandHandler(IPaymentRepository paymentRepository, IMapper mapper, PaymentBusinessRules paymentBusinessRules, BillBusinessRules billBusinessRules, IHttpContextAccessor contextAccessor, IBillReposiotry billRepository, ICreditCardRepository creditCardRepository, IResidentRepository residentRepository)
        {
            _paymentRepository = paymentRepository;
            _mapper = mapper;
            _paymentBusinessRules = paymentBusinessRules;
            _billBusinessRules = billBusinessRules;
            _contextAccessor = contextAccessor;
            _billRepository = billRepository;
            _creditCardRepository = creditCardRepository;
            _residentRepository = residentRepository;
        }

        public string[] Roles => ["user"];

        public async Task<CreatePaymentResponse> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
        {
            
            var billToBePaid = await _billBusinessRules.BillShouldBeExistInDatabase(billId: request.BillId);
            var userId = _contextAccessor.HttpContext.User.GetUserId();

           var dbUser = await _residentRepository.GetByIdAsync(userId);
                if(dbUser is null)
                throw new BusinessException("Kullanıcı bulunamadı");

            var creditCard = await _paymentBusinessRules.AreCardInformationAndBalanceValid(request.CreditCard,Convert.ToDecimal( billToBePaid.Fee));

            var payment = _mapper.Map<Payment>(request);

            creditCard.Amount -= Convert.ToDecimal(billToBePaid.Fee);

            await _paymentRepository.AddAsync(payment);

            billToBePaid.IsPaid = true;

            await _billRepository.UpdateAsync(billToBePaid);
            await _creditCardRepository.UpdateAsync(creditCard);

            return new CreatePaymentResponse
            {
                BillType = billToBePaid.Type.Name,
                Fee = billToBePaid.Fee,
                Period = billToBePaid.Period
            };

        }
    }
}
