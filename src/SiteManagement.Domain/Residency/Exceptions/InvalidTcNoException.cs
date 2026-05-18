using SiteManagement.Domain.Shared.Exceptions;

namespace SiteManagement.Domain.Residency.Exceptions;

/// <summary>
/// Thrown when a candidate string is not a valid Turkish citizenship number
/// (wrong length, non-digit characters, or failing the official checksum).
/// </summary>
public sealed class InvalidTcNoException : DomainException
{
    /// <summary>Creates the exception.</summary>
    public InvalidTcNoException() : base(ResidencyMessageKeys.TcNoInvalid) { }
}
