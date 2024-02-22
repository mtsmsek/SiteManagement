using AutoMapper;
using MediatR;
using SiteManagement.Application.Services.Repositories.Invoices;
using SiteManagement.Domain.Entities.Invoices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Features.Commands.Invoices.Bills.CreateBill
{
    public class CreateBillCommandHandler : IRequestHandler<CreateBillCommand, Guid>
    {
        private readonly IBillReposiotry _billRepository;
        private readonly IMapper _mapper;

        public CreateBillCommandHandler(IBillReposiotry billRepository, IMapper mapper)
        {
            _billRepository = billRepository;
            _mapper = mapper;
        }

        public async Task<Guid> Handle(CreateBillCommand request, CancellationToken cancellationToken)
        {

            var billToAdd = _mapper.Map<Bill>(request);

            await _billRepository.AddAsync(billToAdd);

            return billToAdd.Id;
        }
    }
}
