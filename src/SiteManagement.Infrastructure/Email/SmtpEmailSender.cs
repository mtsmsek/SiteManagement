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

    private readonly SmtpOptions _options = options.Value;

    /// <inheritdoc />
    public async Task SendResidentWelcomeAsync(
        string toEmail,
        string fullName,
        string temporaryPassword,
        CancellationToken ct = default)
    {
        using var message = new MailMessage(_options.FromAddress, toEmail)
        {
            Subject = WelcomeSubjectFormat,
            Body = string.Format(WelcomeBodyTemplate, fullName, toEmail, temporaryPassword),
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
