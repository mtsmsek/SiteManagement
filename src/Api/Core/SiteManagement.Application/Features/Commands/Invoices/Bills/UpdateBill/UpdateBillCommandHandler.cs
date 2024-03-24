using AutoMapper;
using MediatR;
using SiteManagement.Application.Rules.Invoices.Bills;
using SiteManagement.Application.Services.Repositories.Invoices;
using SiteManagement.Domain.Entities.Invoices;



namespace SiteManagement.Application.Features.Commands.Invoices.Bills.UpdateBill
{
    public class UpdateBillCommandHandler : IRequestHandler<UpdateBillCommand, UpdateBillResponse>
    {
        private readonly IBillReposiotry _billRepository;
        private readonly IMapper _mapper;
        private readonly BillBusinessRules _billBusinessRules;

        public UpdateBillCommandHandler(IBillReposiotry billRepository, IMapper mapper, BillBusinessRules billBusinessRules)
        {
            _billRepository = billRepository;
            _mapper = mapper;
            _billBusinessRules = billBusinessRules;
        }

        public async Task<UpdateBillResponse> Handle(UpdateBillCommand request, CancellationToken cancellationToken)
        {
            Bill billToUpdate = await _billBusinessRules.BillShouldBeExistInDatabase(request.Id);
            billToUpdate = _mapper.Map(request, billToUpdate);
            await _billRepository.UpdateAsync(billToUpdate, cancellationToken);
            return _mapper.Map<UpdateBillResponse>(billToUpdate);
        }
    }
}