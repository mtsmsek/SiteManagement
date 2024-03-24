using AutoMapper;
using MediatR;
using SiteManagement.Application.Pagination.Responses;
using SiteManagement.Application.Services.Repositories.Invoices;
using SiteManagement.Domain.Entities.Invoices;

namespace SiteManagement.Application.Features.Commands.Invoices.Bills.CreateBulkBills;

public class CreateBulkBillsCommandHandler : IRequestHandler<CreateBulkBillsCommand,
                                                             PagedViewModel<CreateBulkBillsResponse>>
{
    private readonly IBillReposiotry _billRepository;
    private readonly IMapper _mapper;

    public CreateBulkBillsCommandHandler(IBillReposiotry billRepository, IMapper mapper)
    {
        _billRepository = billRepository;
        _mapper = mapper;
    }

    public async Task<PagedViewModel<CreateBulkBillsResponse>> Handle(CreateBulkBillsCommand request, CancellationToken cancellationToken)
    {
        List<Bill> billsToAdd = new();

        foreach(var bill in request.Bills)
        {
           var b = _mapper.Map<Bill>(bill);
            billsToAdd.Add(b);  
        }
        await _billRepository.AddRangeAsync(billsToAdd);

        return _mapper.Map<PagedViewModel<CreateBulkBillsResponse>>(billsToAdd);
    }
}
