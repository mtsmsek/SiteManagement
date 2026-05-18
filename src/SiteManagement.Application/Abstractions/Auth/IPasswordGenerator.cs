namespace SiteManagement.Application.Abstractions.Auth;

/// <summary>
/// Port for generating one-off passwords (resident welcome emails, admin
/// resets). Concrete implementation lives in Infrastructure; the
/// Application layer stays free of cryptography APIs.
/// </summary>
public interface IPasswordGenerator
{
    /// <summary>Generates a random password that satisfies the Identity password policy.</summary>
    string Generate();
}
