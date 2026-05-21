using MediatR;
using SiteManagement.Application.Abstractions.Email;
using SiteManagement.Application.Abstractions.Events;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Application.Residency.Queries;
using SiteManagement.Domain.Billing.Events;

namespace SiteManagement.Application.Billing.EventHandlers;

/// <summary>
/// Builds one dues notification per resident (items grouped by resident, their
/// amounts summed, so a resident with several apartments isn't notified twice)
/// and hands the whole batch to the email sender in a single call. Runs inside
/// the closing command's transaction (see EfUnitOfWork's dispatch loop);
/// moving delivery to after-commit / an outbox is a W6 concern.
/// </summary>
public sealed class DuesPeriodClosedEventHandler(
    IDuesPeriodRepository duesPeriodRepository,
    IResidentQueries residentQueries,
    IEmailSender emailSender)
    : INotificationHandler<DomainEventNotification<DuesPeriodClosed>>
{
    private const string DuesKind = "Dues";

    private readonly IDuesPeriodRepository _duesPeriodRepository = duesPeriodRepository;
    private readonly IResidentQueries _residentQueries = residentQueries;
    private readonly IEmailSender _emailSender = emailSender;

    /// <inheritdoc />
    public async Task Handle(
        DomainEventNotification<DuesPeriodClosed> notification,
        CancellationToken cancellationToken)
    {
        var period = await _duesPeriodRepository.GetByIdAsync(notification.DomainEvent.DuesPeriodId, cancellationToken);
        if (period is null || period.Items.Count == 0)
        {
            return;
        }

        var totalsByResident = period.Items
            .GroupBy(i => i.ResidentId)
            .ToDictionary(g => g.Key, g => g.Sum(i => i.Amount.Amount));

        var emails = await _residentQueries.GetEmailsByIdsAsync(totalsByResident.Keys, cancellationToken);
        var month = period.Month.ToString();

        var notifications = totalsByResident
            .Where(t => emails.ContainsKey(t.Key))
            .Select(t => new BillingNotification(emails[t.Key], DuesKind, month, t.Value))
            .ToList();

        await _emailSender.SendBillingNotificationsAsync(notifications, cancellationToken);
    }
}
