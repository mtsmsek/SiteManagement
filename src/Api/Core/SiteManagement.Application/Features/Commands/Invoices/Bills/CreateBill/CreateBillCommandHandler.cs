using AutoMapper;
using MediatR;
using SiteManagement.Application.Rules.Buildings.Apartments;
using SiteManagement.Application.Rules.Invoices.Bills;
using SiteManagement.Application.Services.Repositories.Invoices;
using SiteManagement.Domain.Entities.Invoices;

namespace SiteManagement.Application.Features.Commands.Invoices.Bills.CreateBill;

public class CreateBillCommandHandler : IRequestHandler<CreateBillCommand, Guid>
{
    private readonly IBillReposiotry _billRepository;
    private readonly IMapper _mapper;
    private readonly BillBusinessRules _billBusinessRules;
    private readonly ApartmentBusinessRules _apartmentBusinessRules;

    public CreateBillCommandHandler(IBillReposiotry billRepository, IMapper mapper, BillBusinessRules billBusinessRules, ApartmentBusinessRules apartmentBusinessRules)
    {
        _billRepository = billRepository;
        _mapper = mapper;
        _billBusinessRules = billBusinessRules;
        _apartmentBusinessRules = apartmentBusinessRules;
    }

    public async Task<Guid> Handle(CreateBillCommand request, CancellationToken cancellationToken)
    {
        await _apartmentBusinessRules.ApartmentShouldExistInDatabase(request.ApartmentId, cancellationToken);

        await _billBusinessRules.ShouldBeOneBillTypeForSameApartmentForTheSamePeriod(request.ApartmentId,
                                                                                     request.Type,
                                                                                     request.Month,
                                                                                     request.Year);

        var billToAdd = _mapper.Map<Bill>(request);

        await _billRepository.AddAsync(billToAdd);

        //TODO - Add RabbitMQ implementation to send residents a notification

        return billToAdd.Id;
    }
}
