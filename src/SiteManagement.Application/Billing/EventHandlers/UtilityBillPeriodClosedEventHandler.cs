using MediatR;
using SiteManagement.Application.Abstractions.Email;
using SiteManagement.Application.Abstractions.Events;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Application.Residency.Queries;
using SiteManagement.Domain.Billing.Events;

namespace SiteManagement.Application.Billing.EventHandlers;

/// <summary>
/// Builds one utility notification per resident and hands the batch to the
/// email sender in a single call — the utility counterpart of
/// <see cref="DuesPeriodClosedEventHandler"/>. Runs inside the closing
/// command's transaction.
/// </summary>
public sealed class UtilityBillPeriodClosedEventHandler(
    IUtilityBillPeriodRepository utilityBillPeriodRepository,
    IResidentQueries residentQueries,
    IEmailSender emailSender)
    : INotificationHandler<DomainEventNotification<UtilityBillPeriodClosed>>
{
    private readonly IUtilityBillPeriodRepository _utilityBillPeriodRepository = utilityBillPeriodRepository;
    private readonly IResidentQueries _residentQueries = residentQueries;
    private readonly IEmailSender _emailSender = emailSender;

    /// <inheritdoc />
    public async Task Handle(
        DomainEventNotification<UtilityBillPeriodClosed> notification,
        CancellationToken cancellationToken)
    {
        var period = await _utilityBillPeriodRepository.GetByIdAsync(
            notification.DomainEvent.UtilityBillPeriodId, cancellationToken);
        if (period is null || period.Items.Count == 0)
        {
            return;
        }

        var totalsByResident = period.Items
            .GroupBy(i => i.ResidentId)
            .ToDictionary(g => g.Key, g => g.Sum(i => i.Amount.Amount));

        var emails = await _residentQueries.GetEmailsByIdsAsync(totalsByResident.Keys, cancellationToken);
        var kind = period.UtilityType.ToString();
        var month = period.Month.ToString();

        var notifications = totalsByResident
            .Where(t => emails.ContainsKey(t.Key))
            .Select(t => new BillingNotification(emails[t.Key], kind, month, t.Value))
            .ToList();

        await _emailSender.SendBillingNotificationsAsync(notifications, cancellationToken);
    }
}
