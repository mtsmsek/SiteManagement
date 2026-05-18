using SiteManagement.Domain.Shared.Exceptions;

namespace SiteManagement.Domain.Residency.Exceptions;

/// <summary>
/// Thrown when an email value does not match the canonical shape or exceeds
/// <see cref="ResidencyLimits.EmailMaxLength"/>.
/// </summary>
public sealed class InvalidEmailException : DomainException
{
    /// <summary>Creates the exception.</summary>
    public InvalidEmailException(string rawValue)
        : base(ResidencyMessageKeys.EmailInvalid, rawValue) { }
}
