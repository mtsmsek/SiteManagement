namespace SiteManagement.Application.Abstractions.Email;

/// <summary>
/// Application-facing port for outbound transactional emails. The MailHog-
/// backed dev implementation prints captured emails into the MailHog UI;
/// production uses a real SMTP provider via the same port.
/// </summary>
public interface IEmailSender
{
    /// <summary>Sends the resident welcome email containing the generated password.</summary>
    Task SendResidentWelcomeAsync(
        string toEmail,
        string fullName,
        string temporaryPassword,
        CancellationToken ct = default);

    /// <summary>
    /// Sends a billing notification to each recipient in one call. The handler
    /// states <em>what</em> to notify; how the batch is delivered (sequential,
    /// parallel, …) is the sender implementation's concern, keeping the
    /// per-recipient send loop out of the Application layer.
    /// </summary>
    Task SendBillingNotificationsAsync(
        IReadOnlyCollection<BillingNotification> notifications,
        CancellationToken ct = default);
}

/// <summary>
/// One resident's billing notification: their email plus the kind, month, and
/// total amount being announced.
/// </summary>
/// <param name="ToEmail">Recipient address.</param>
/// <param name="BillingKind">"Dues" or the utility name (Electricity, …).</param>
/// <param name="Month">The billed month as "yyyy-MM".</param>
/// <param name="Amount">The total the resident owes for that kind + month.</param>
public sealed record BillingNotification(string ToEmail, string BillingKind, string Month, decimal Amount);
