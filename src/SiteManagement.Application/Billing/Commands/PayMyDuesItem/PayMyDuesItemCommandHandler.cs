using MediatR;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Application.Billing.Services;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Domain.Billing;

namespace SiteManagement.Application.Billing.Commands.PayMyDuesItem;

/// <summary>
/// Pays one of the current resident's own dues items. Role and ownership are
/// already enforced by the pipeline (the command is an
/// <c>IOwnedBillItemRequest</c>), so this handler is pure billing work and
/// matches the admin path: charge through the shared
/// <see cref="IBillItemPaymentService"/> first, mark paid only on success
/// (decline → 402, item stays unpaid; retry is idempotent).
/// </summary>
public sealed class PayMyDuesItemCommandHandler(
    IDuesPeriodRepository duesPeriodRepository,
    IBillItemPaymentService payment,
    IUnitOfWork unitOfWork)
    : IRequestHandler<PayMyDuesItemCommand>
{
    private readonly IDuesPeriodRepository _duesPeriodRepository = duesPeriodRepository;
    private readonly IBillItemPaymentService _payment = payment;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    /// <inheritdoc />
    public async Task Handle(PayMyDuesItemCommand request, CancellationToken cancellationToken)
    {
        var period = await _duesPeriodRepository.GetByIdAsync(request.DuesPeriodId, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(DuesPeriod), request.DuesPeriodId);

        var item = period.Items.FirstOrDefault(i => i.Id == request.ItemId)
            ?? throw new EntityNotFoundException(nameof(DuesItem), request.ItemId);

        await _payment.ChargeOrThrowAsync(
            BillItemReference.ForDuesItem(item.Id),
            item.Amount.Amount,
            new CardDetails(request.CardNumber, request.Cvv, request.ExpiryYear, request.ExpiryMonth),
            cancellationToken);

        period.MarkItemPaid(item.Id);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
