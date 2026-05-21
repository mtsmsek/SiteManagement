using MediatR;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Domain.Billing;

namespace SiteManagement.Application.Billing.Commands.CloseUtilityBillPeriod;

/// <summary>
/// Closes a utility bill period. The domain raises <c>UtilityBillPeriodClosed</c>;
/// its notification handler emails residents in a follow-up step, all inside the
/// transaction TransactionBehavior wraps the command in.
/// </summary>
public sealed class CloseUtilityBillPeriodCommandHandler(
    IUtilityBillPeriodRepository utilityBillPeriodRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CloseUtilityBillPeriodCommand>
{
    private readonly IUtilityBillPeriodRepository _utilityBillPeriodRepository = utilityBillPeriodRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    /// <inheritdoc />
    public async Task Handle(CloseUtilityBillPeriodCommand request, CancellationToken cancellationToken)
    {
        var period = await _utilityBillPeriodRepository.GetByIdAsync(request.UtilityBillPeriodId, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(UtilityBillPeriod), request.UtilityBillPeriodId);

        period.Close();

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
