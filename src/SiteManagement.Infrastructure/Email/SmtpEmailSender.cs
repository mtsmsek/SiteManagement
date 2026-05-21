using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using SiteManagement.Application.Abstractions.Email;

namespace SiteManagement.Infrastructure.Email;

/// <summary>
/// SMTP-backed <see cref="IEmailSender"/>. In dev runs against MailHog
/// (no TLS, no credentials); in production binds to a real SMTP provider
/// via the same options.
/// </summary>
public sealed class SmtpEmailSender(IOptions<SmtpOptions> options) : IEmailSender
{
    private const string WelcomeSubjectFormat = "Welcome to SiteManagement — your temporary password";
    private const string WelcomeBodyTemplate =
        """
        Hello {0},

        An admin has created a SiteManagement account on your behalf.

        Email:    {1}
        Password: {2}

        Please log in and change the password as soon as possible.
        """;

    private const string BillingSubjectTemplate = "SiteManagement — {0} for {1}";
    private const string BillingBodyTemplate =
        """
        Hello,

        Your {0} for {1} has been finalised.

        Amount due: {2:0.00} TRY

        Please log in to view and pay your balance.
        """;

    private readonly SmtpOptions _options = options.Value;

    /// <inheritdoc />
    public Task SendResidentWelcomeAsync(
        string toEmail,
        string fullName,
        string temporaryPassword,
        CancellationToken ct = default)
        => SendAsync(
            toEmail,
            WelcomeSubjectFormat,
            string.Format(WelcomeBodyTemplate, fullName, toEmail, temporaryPassword),
            ct);

    /// <inheritdoc />
    public async Task SendBillingNotificationsAsync(
        IReadOnlyCollection<BillingNotification> notifications,
        CancellationToken ct = default)
    {
        // One SMTP message per recipient (contents differ); the per-recipient
        // loop lives here, in the sender, not in the Application handler.
        foreach (var n in notifications)
        {
            await SendAsync(
                n.ToEmail,
                string.Format(BillingSubjectTemplate, n.BillingKind, n.Month),
                string.Format(BillingBodyTemplate, n.BillingKind, n.Month, n.Amount),
                ct);
        }
    }

    /// <summary>Sends one plain-text message through the configured SMTP host.</summary>
    private async Task SendAsync(string toEmail, string subject, string body, CancellationToken ct)
    {
        using var message = new MailMessage(_options.FromAddress, toEmail)
        {
            Subject = subject,
            Body = body,
            IsBodyHtml = false,
        };

        using var client = new SmtpClient(_options.Host, _options.Port)
        {
            EnableSsl = _options.EnableSsl,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
        };

        if (!string.IsNullOrEmpty(_options.Username))
        {
            client.Credentials = new NetworkCredential(_options.Username, _options.Password);
        }

        await client.SendMailAsync(message, ct);
    }
}
