using AutoMapper;
using MediatR;
using SiteManagement.Application.Rules.Invoices.Bills;
using SiteManagement.Application.Services.Repositories.Invoices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Features.Commands.Invoices.Bills.DeleteBill.HardDelete
{
    public class HardDeleteBillCommandHandler : IRequestHandler<HardDeleteBillCommand, Guid>
    {
        private readonly IBillReposiotry _billRepository;
        private readonly BillBusinessRules _billBusinessRules;

        public HardDeleteBillCommandHandler(IBillReposiotry billRepository, BillBusinessRules billBusinessRules)
        {
            _billRepository = billRepository;
            _billBusinessRules = billBusinessRules;
        }

        public async Task<Guid> Handle(HardDeleteBillCommand request, CancellationToken cancellationToken)
        {
            var billToDelete = await _billBusinessRules.BillShouldBeExistInDatabase(request.Id);

            await _billRepository.DeleteAsync(entity: billToDelete,
                                        isPermenant: true,
                                        cancellationToken: cancellationToken);

            return request.Id;
            //TODO - Add update bill when payment service implemented
        }


    }
}
