using SiteManagement.Domain.Shared.Exceptions;

namespace SiteManagement.Domain.Residency.Exceptions;

/// <summary>
/// Thrown when a phone number does not match the Turkish format
/// (10 trunk digits, optionally prefixed with <c>0</c> or <c>+90</c>).
/// </summary>
public sealed class InvalidPhoneNumberException : DomainException
{
    /// <summary>Creates the exception.</summary>
    public InvalidPhoneNumberException(string rawValue)
        : base(ResidencyMessageKeys.PhoneNumberInvalid, rawValue) { }
}
