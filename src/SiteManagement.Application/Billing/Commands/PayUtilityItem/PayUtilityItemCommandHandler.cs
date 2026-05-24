using MediatR;
using SiteManagement.Application.Abstractions.Payments;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Application.Shared.Resources;
using SiteManagement.Domain.Billing;

namespace SiteManagement.Application.Billing.Commands.PayUtilityItem;

/// <summary>
/// Charges a utility bill item through the payment gateway, then marks it paid
/// only on success. Charge first, persist second: if the charge is declined the
/// item stays unpaid (a <see cref="PaymentRejectedException"/> → 402); if the
/// charge succeeds but the local save somehow fails, the deterministic
/// idempotency key makes a retry safe — the gateway returns the prior success
/// instead of charging again. The gateway resolves which card from its own
/// seeded data; this handler only supplies the amount + keys.
/// </summary>
public sealed class PayUtilityItemCommandHandler(
    IUtilityBillPeriodRepository utilityBillPeriodRepository,
    IPaymentGateway paymentGateway,
    IUnitOfWork unitOfWork)
    : IRequestHandler<PayUtilityItemCommand>
{
    private readonly IUtilityBillPeriodRepository _utilityBillPeriodRepository = utilityBillPeriodRepository;
    private readonly IPaymentGateway _paymentGateway = paymentGateway;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    /// <inheritdoc />
    public async Task Handle(PayUtilityItemCommand request, CancellationToken cancellationToken)
    {
        var period = await _utilityBillPeriodRepository.GetByIdAsync(request.UtilityBillPeriodId, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(UtilityBillPeriod), request.UtilityBillPeriodId);

        var item = period.Items.FirstOrDefault(i => i.Id == request.ItemId)
            ?? throw new EntityNotFoundException(nameof(UtilityBillItem), request.ItemId);

        var charge = new ChargeRequest(
            IdempotencyKey: $"utility-item:{item.Id}",
            CardNumber: request.CardNumber,
            Cvv: request.Cvv,
            ExpiryYear: request.ExpiryYear,
            ExpiryMonth: request.ExpiryMonth,
            Amount: item.Amount.Amount,
            Reference: $"utility-item:{item.Id}");

        var result = await _paymentGateway.ChargeAsync(charge, cancellationToken);
        if (!result.Succeeded)
        {
            // Carry the message key; the Api middleware localizes it (same
            // contract as BusinessRuleViolationException).
            throw new PaymentRejectedException(
                ErrorMessageKeys.PaymentRejected,
                result.FailureReason ?? "rejected");
        }

        period.MarkItemPaid(item.Id);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
