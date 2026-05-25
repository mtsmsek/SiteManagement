using MediatR;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Application.Billing.Services;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Domain.Billing;
using SiteManagement.Domain.Shared.ValueObjects;

namespace SiteManagement.Application.Billing.Commands.ChangeDuesAmount;

/// <summary>
/// Re-rates an open dues period to a new per-apartment amount. The aggregate
/// returns the over-payments produced on any already-paid items; the credit
/// service posts them to the residents' accounts in the same transaction
/// (TransactionBehavior wraps the save), so the correction and the credit
/// commit atomically.
/// </summary>
public sealed class ChangeDuesAmountCommandHandler(
    IDuesPeriodRepository duesPeriodRepository,
    IResidentCreditService creditService,
    IUnitOfWork unitOfWork)
    : IRequestHandler<ChangeDuesAmountCommand>
{
    private readonly IDuesPeriodRepository _duesPeriodRepository = duesPeriodRepository;
    private readonly IResidentCreditService _creditService = creditService;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    /// <inheritdoc />
    public async Task Handle(ChangeDuesAmountCommand request, CancellationToken cancellationToken)
    {
        var period = await _duesPeriodRepository.GetByIdAsync(request.DuesPeriodId, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(DuesPeriod), request.DuesPeriodId);

        var credits = period.ChangePerApartmentAmount(Money.Of(request.PerApartmentAmount));

        await _creditService.ApplyCreditsAsync(credits, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
