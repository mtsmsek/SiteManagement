using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;
using SiteManagement.Application.Rules.Commons;
using SiteManagement.Application.Services.Repositories.Payments;
using SiteManagement.Domain.Entities.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Rules.Payments
{
    public class PaymentBusinessRules : BaseBusinessRules
    {
        private readonly ICreditCardRepository _creditCardRepository;

        public PaymentBusinessRules(ICreditCardRepository creditCardRepository)
        {
            _creditCardRepository = creditCardRepository;
        }

        public async Task<CreditCard> AreCardInformationAndBalanceValid(CreditCard creditCard, decimal billAmount)
        {
            var dbCreditCard = await _creditCardRepository.GetSingleAsync(predicate: card => card.NameOnCard == creditCard.NameOnCard &&
                                                                    card.CardNumber == creditCard.CardNumber &&
                                                                    card.CVCNumber == creditCard.CVCNumber &&
                                                                    card.ExpireDate == creditCard.ExpireDate);
            ///todo -- remove magic string
            if (dbCreditCard is null)
                throw new BusinessException("Kart bilgileri yanlış");

            if (dbCreditCard.Amount < billAmount)
                throw new BusinessException("Yetersiz bakiye");

            return dbCreditCard;
        }

    }
}
