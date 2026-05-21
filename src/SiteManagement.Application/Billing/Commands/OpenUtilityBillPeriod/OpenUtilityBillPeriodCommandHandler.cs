using MediatR;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Domain.Billing;
using SiteManagement.Domain.Billing.ValueObjects;
using SiteManagement.Domain.Shared.ValueObjects;

namespace SiteManagement.Application.Billing.Commands.OpenUtilityBillPeriod;

/// <summary>
/// Opens a new utility bill period. The domain value objects (BillingMonth, Money)
/// re-validate the month and total amount; TransactionBehavior wraps the save.
/// </summary>
public sealed class OpenUtilityBillPeriodCommandHandler(
    IUtilityBillPeriodRepository utilityBillPeriodRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<OpenUtilityBillPeriodCommand, OpenUtilityBillPeriodResult>
{
    private readonly IUtilityBillPeriodRepository _utilityBillPeriodRepository = utilityBillPeriodRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    /// <inheritdoc />
    public async Task<OpenUtilityBillPeriodResult> Handle(OpenUtilityBillPeriodCommand request, CancellationToken cancellationToken)
    {
        var period = UtilityBillPeriod.Open(
            request.SiteId,
            BillingMonth.Of(request.Year, request.Month),
            request.UtilityType,
            Money.Of(request.TotalAmount));

        await _utilityBillPeriodRepository.AddAsync(period, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new OpenUtilityBillPeriodResult(period.Id);
    }
}
