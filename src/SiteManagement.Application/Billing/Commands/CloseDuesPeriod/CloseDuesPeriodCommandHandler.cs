using MediatR;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Domain.Billing;

namespace SiteManagement.Application.Billing.Commands.CloseDuesPeriod;

/// <summary>
/// Closes a dues period. The domain raises <c>DuesPeriodClosed</c>; its
/// notification handler emails residents in a follow-up step, all inside the
/// transaction TransactionBehavior wraps the command in.
/// </summary>
public sealed class CloseDuesPeriodCommandHandler(
    IDuesPeriodRepository duesPeriodRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CloseDuesPeriodCommand>
{
    private readonly IDuesPeriodRepository _duesPeriodRepository = duesPeriodRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    /// <inheritdoc />
    public async Task Handle(CloseDuesPeriodCommand request, CancellationToken cancellationToken)
    {
        var period = await _duesPeriodRepository.GetByIdAsync(request.DuesPeriodId, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(DuesPeriod), request.DuesPeriodId);

        period.Close();

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
