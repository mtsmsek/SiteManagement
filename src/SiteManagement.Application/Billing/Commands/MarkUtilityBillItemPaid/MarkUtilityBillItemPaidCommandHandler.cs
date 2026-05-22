using MediatR;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Domain.Billing;

namespace SiteManagement.Application.Billing.Commands.MarkUtilityBillItemPaid;

/// <summary>
/// Loads the utility bill period and marks one of its items paid. The period
/// enforces that the item belongs to it; TransactionBehavior wraps the save.
/// </summary>
public sealed class MarkUtilityBillItemPaidCommandHandler(
    IUtilityBillPeriodRepository utilityBillPeriodRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<MarkUtilityBillItemPaidCommand>
{
    private readonly IUtilityBillPeriodRepository _utilityBillPeriodRepository = utilityBillPeriodRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    /// <inheritdoc />
    public async Task Handle(MarkUtilityBillItemPaidCommand request, CancellationToken cancellationToken)
    {
        var period = await _utilityBillPeriodRepository.GetByIdAsync(request.UtilityBillPeriodId, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(UtilityBillPeriod), request.UtilityBillPeriodId);

        period.MarkItemPaid(request.ItemId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
