using MediatR;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Domain.Billing;
using SiteManagement.Domain.Billing.ValueObjects;
using SiteManagement.Domain.Shared.ValueObjects;

namespace SiteManagement.Application.Billing.Commands.OpenDuesPeriod;

/// <summary>
/// Opens a new dues period. The domain value objects (BillingMonth, Money)
/// re-validate the month and amount; TransactionBehavior wraps the save.
/// </summary>
public sealed class OpenDuesPeriodCommandHandler(
    IDuesPeriodRepository duesPeriodRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<OpenDuesPeriodCommand, OpenDuesPeriodResult>
{
    private readonly IDuesPeriodRepository _duesPeriodRepository = duesPeriodRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    /// <inheritdoc />
    public async Task<OpenDuesPeriodResult> Handle(OpenDuesPeriodCommand request, CancellationToken cancellationToken)
    {
        var period = DuesPeriod.Open(
            request.SiteId,
            BillingMonth.Of(request.Year, request.Month),
            Money.Of(request.PerApartmentAmount));

        await _duesPeriodRepository.AddAsync(period, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new OpenDuesPeriodResult(period.Id);
    }
}
