using MediatR;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Domain.Billing;

namespace SiteManagement.Application.Billing.Commands.MarkDuesItemPaid;

/// <summary>
/// Loads the dues period and marks one of its items paid. The period enforces
/// that the item belongs to it; TransactionBehavior wraps the save.
/// </summary>
public sealed class MarkDuesItemPaidCommandHandler(
    IDuesPeriodRepository duesPeriodRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<MarkDuesItemPaidCommand>
{
    private readonly IDuesPeriodRepository _duesPeriodRepository = duesPeriodRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    /// <inheritdoc />
    public async Task Handle(MarkDuesItemPaidCommand request, CancellationToken cancellationToken)
    {
        var period = await _duesPeriodRepository.GetByIdAsync(request.DuesPeriodId, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(DuesPeriod), request.DuesPeriodId);

        period.MarkItemPaid(request.ItemId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
