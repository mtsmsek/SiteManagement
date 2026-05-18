namespace SiteManagement.Infrastructure.Email;

/// <summary>
/// SMTP options bound from the <c>Smtp</c> configuration section. The
/// docker-compose stack sets these to MailHog defaults; production
/// deployments override them with their real provider.
/// </summary>
public class SmtpOptions
{
    /// <summary>Configuration section name.</summary>
    public const string SectionName = "Smtp";

    /// <summary>SMTP host (e.g. <c>mailhog</c> in compose, <c>smtp.sendgrid.net</c> in prod).</summary>
    public string Host { get; init; } = "localhost";

    /// <summary>SMTP port. MailHog listens on 1025.</summary>
    public int Port { get; init; } = 1025;

    /// <summary>Whether to use TLS. MailHog does not support TLS.</summary>
    public bool EnableSsl { get; init; } = false;

    /// <summary>Optional SMTP auth username.</summary>
    public string? Username { get; init; }

    /// <summary>Optional SMTP auth password.</summary>
    public string? Password { get; init; }

    /// <summary>From address used by every outbound mail.</summary>
    public string FromAddress { get; init; } = "no-reply@sitemanagement.local";
}
