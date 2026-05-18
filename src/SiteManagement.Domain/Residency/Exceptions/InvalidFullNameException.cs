using SiteManagement.Domain.Shared.Exceptions;

namespace SiteManagement.Domain.Residency.Exceptions;

/// <summary>
/// Thrown when a first or last name is empty / whitespace / longer than
/// <see cref="ResidencyLimits.NameComponentMaxLength"/>.
/// </summary>
public sealed class InvalidFullNameException : DomainException
{
    /// <summary>Creates the exception.</summary>
    public InvalidFullNameException()
        : base(ResidencyMessageKeys.FullNameInvalid, ResidencyLimits.NameComponentMaxLength) { }
}
