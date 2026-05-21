using System.Collections.Concurrent;
using SiteManagement.Application.Abstractions.Email;

namespace SiteManagement.E2E.Tests.Infrastructure;

/// <summary>
/// Test-only <see cref="IEmailSender"/> that records every dispatch into a
/// thread-safe queue instead of touching SMTP. Lets integration tests
/// assert on the welcome email's recipient / temporary password without a
/// MailHog container.
/// </summary>
public sealed class RecordingEmailSender : IEmailSender
{
    private readonly ConcurrentQueue<RecordedEmail> _sent = new();

    /// <summary>Every email recorded since the last <see cref="Clear"/> call.</summary>
    public IReadOnlyCollection<RecordedEmail> Sent => _sent.ToArray();

    private readonly ConcurrentQueue<RecordedBillingNotification> _billingNotifications = new();

    /// <summary>Every billing notification recorded since the last <see cref="Clear"/>.</summary>
    public IReadOnlyCollection<RecordedBillingNotification> BillingNotifications => _billingNotifications.ToArray();

    /// <inheritdoc />
    public Task SendResidentWelcomeAsync(
        string toEmail,
        string fullName,
        string temporaryPassword,
        CancellationToken ct = default)
    {
        _sent.Enqueue(new RecordedEmail(toEmail, fullName, temporaryPassword, DateTime.UtcNow));
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task SendBillingNotificationsAsync(
        IReadOnlyCollection<BillingNotification> notifications,
        CancellationToken ct = default)
    {
        foreach (var n in notifications)
        {
            _billingNotifications.Enqueue(
                new RecordedBillingNotification(n.ToEmail, n.BillingKind, n.Month, n.Amount));
        }

        return Task.CompletedTask;
    }

    /// <summary>Drops every recorded email — called between tests.</summary>
    public void Clear()
    {
        while (_sent.TryDequeue(out _)) { }
        while (_billingNotifications.TryDequeue(out _)) { }
    }
}

/// <summary>One captured outbound email.</summary>
public sealed record RecordedEmail(string ToEmail, string FullName, string TemporaryPassword, DateTime SentAtUtc);

/// <summary>One captured billing notification.</summary>
public sealed record RecordedBillingNotification(string ToEmail, string BillingKind, string Month, decimal Amount);
