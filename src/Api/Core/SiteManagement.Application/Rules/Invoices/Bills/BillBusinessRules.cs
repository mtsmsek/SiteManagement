using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;
using SiteManagement.Application.Rules.Buildings.Apartments;
using SiteManagement.Application.Rules.Commons;
using SiteManagement.Application.Services.Repositories.Buildings;
using SiteManagement.Application.Services.Repositories.Invoices;
using SiteManagement.Domain.Entities.Invoices;
using SiteManagement.Domain.Enumarations.Invoices;
using SiteManagement.Domain.Utulity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Rules.Invoices.Bills
{
    public class BillBusinessRules : BaseBusinessRules
    {

        private readonly IBillReposiotry _billRepository;
        private readonly IApartmentRepository _apartmentRepository;
        private readonly ApartmentBusinessRules _apartmentBusinessRules;

        public BillBusinessRules(IBillReposiotry billRepository, ApartmentBusinessRules apartmentBusinessRules)
        {
            _billRepository = billRepository;
            _apartmentBusinessRules = apartmentBusinessRules;
        }
        public async Task<Bill> BillShouldBeExistInDatabase(Guid billId)
        {
            var bill = await _billRepository.GetByIdAsync(billId);
            Ensure.NotNull(bill, new BusinessException("Aradığınız fatura veri tabanında mevcut değil"));

            return bill;
        }
        public async Task ShouldBeOneBillTypeForSameApartmentForTheSamePeriod(Guid apartmentId,BillType billType ,Month month, int year)
        {
            await ApartmentShouldBeExistInDatabase(apartmentId);

           var isBillExist =  await _billRepository.AnyAsync(predicate: 
                                                        bill => bill.ApartmentId == apartmentId &&
                                                        bill.Type == billType &&
                                                        bill.Month == month &&
                                                        bill.Year == year);
            if (isBillExist)
                throw new BusinessException("Aynı dönem için bir fatura türüne ait en fazla bir fatura kesebilirsiniz.");

        }

        public async Task ApartmentShouldBeExistInDatabase(Guid apartmentId)
        {
            await _apartmentBusinessRules.ApartmentShouldExistInDatabase(apartmentId);
        }




    }
}
