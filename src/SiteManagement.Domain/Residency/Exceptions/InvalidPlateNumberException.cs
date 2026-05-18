using SiteManagement.Domain.Shared.Exceptions;

namespace SiteManagement.Domain.Residency.Exceptions;

/// <summary>
/// Thrown when a license plate does not match the Turkish format
/// (NN[A-Z]{1,3}NNNN, with NN a valid province code).
/// </summary>
public sealed class InvalidPlateNumberException : DomainException
{
    /// <summary>Creates the exception.</summary>
    public InvalidPlateNumberException(string rawValue)
        : base(ResidencyMessageKeys.PlateNumberInvalid, rawValue) { }
}
