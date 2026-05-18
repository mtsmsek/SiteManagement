namespace SiteManagement.Infrastructure.Auth;

/// <summary>
/// Configuration for the bootstrap admin user created on startup. Read from
/// the <c>Admin</c> configuration section (env-prefixed in compose / Railway).
/// The startup seeder skips when either field is missing, so local dev runs
/// without any bootstrap admin if the operator hasn't set one up.
/// </summary>
/// <remarks>
/// No <c>[EmailAddress]</c> annotation on <see cref="BootstrapEmail"/> on
/// purpose — DataAnnotations runs at bind time, so a deliberately-empty
/// value (the "no bootstrap admin in this environment" signal) would
/// otherwise blow up startup. Email shape is validated by
/// <c>UserManager.CreateAsync</c> when the seeder actually uses the value.
/// </remarks>
public class AdminBootstrapOptions
{
    /// <summary>Configuration section name bound at startup.</summary>
    public const string SectionName = "Admin";

    /// <summary>Email + login of the bootstrap admin. Empty disables seeding.</summary>
    public string? BootstrapEmail { get; init; }

    /// <summary>Password for the bootstrap admin. Empty disables seeding.</summary>
    public string? BootstrapPassword { get; init; }

    /// <summary>Full name shown in UI + embedded in tokens.</summary>
    public string BootstrapFullName { get; init; } = "Bootstrap Admin";

    /// <summary>True when both email and password are present.</summary>
    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(BootstrapEmail) && !string.IsNullOrWhiteSpace(BootstrapPassword);
}
