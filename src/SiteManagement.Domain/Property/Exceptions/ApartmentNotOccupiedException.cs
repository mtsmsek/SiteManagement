using SiteManagement.Domain.Shared.Exceptions;

namespace SiteManagement.Domain.Property.Exceptions;

/// <summary>
/// Thrown when a caller tries to vacate an apartment that is already empty.
/// </summary>
public sealed class ApartmentNotOccupiedException : DomainException
{
    /// <summary>Creates the exception for the given apartment id.</summary>
    public ApartmentNotOccupiedException(Guid apartmentId)
        : base(PropertyMessageKeys.ApartmentNotOccupied, apartmentId)
    {
    }
}
