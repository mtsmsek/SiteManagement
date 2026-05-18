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
}
