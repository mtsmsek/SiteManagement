using MediatR;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Application.Billing.Services;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Domain.Billing;
using SiteManagement.Domain.Shared.ValueObjects;

namespace SiteManagement.Application.Billing.Commands.ChangeUtilityBillAmount;

/// <summary>
/// Re-splits an open utility bill period to a new total. The aggregate returns
/// the over-payments produced on any already-paid items; the credit service
/// posts them to the residents' accounts in the same transaction
/// (TransactionBehavior wraps the save), so the correction and the credit
/// commit atomically.
/// </summary>
public sealed class ChangeUtilityBillAmountCommandHandler(
    IUtilityBillPeriodRepository utilityBillPeriodRepository,
    IResidentCreditService creditService,
    IUnitOfWork unitOfWork)
    : IRequestHandler<ChangeUtilityBillAmountCommand>
{
    private readonly IUtilityBillPeriodRepository _utilityBillPeriodRepository = utilityBillPeriodRepository;
    private readonly IResidentCreditService _creditService = creditService;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    /// <inheritdoc />
    public async Task Handle(ChangeUtilityBillAmountCommand request, CancellationToken cancellationToken)
    {
        var period = await _utilityBillPeriodRepository.GetByIdAsync(request.UtilityBillPeriodId, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(UtilityBillPeriod), request.UtilityBillPeriodId);

        var credits = period.ChangeTotalAmount(Money.Of(request.TotalAmount));

        await _creditService.ApplyCreditsAsync(credits, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
