using SiteManagement.Domain.Shared.Exceptions;

namespace SiteManagement.Domain.Property.Exceptions;

/// <summary>
/// Thrown when an <c>ApartmentType</c> is created from a string that does not
/// match the <c>N+M</c> format expected by the domain (e.g. <c>"2+1"</c>).
/// </summary>
public sealed class InvalidApartmentTypeException : DomainException
{
    /// <summary>Creates the exception for the offending input.</summary>
    /// <param name="rawValue">The raw value that failed parsing.</param>
    public InvalidApartmentTypeException(string rawValue)
        : base(PropertyMessageKeys.ApartmentTypeInvalid, rawValue)
    {
    }
}
