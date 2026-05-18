using SiteManagement.Domain.Shared.Exceptions;

namespace SiteManagement.Domain.Property.Exceptions;

/// <summary>
/// Thrown when a caller tries to occupy an apartment that is already in the
/// <c>Occupied</c> state. Vacate it first via <c>MarkAsEmpty()</c>.
/// </summary>
public sealed class ApartmentAlreadyOccupiedException : DomainException
{
    /// <summary>Creates the exception for the given apartment id.</summary>
    public ApartmentAlreadyOccupiedException(Guid apartmentId)
        : base(PropertyMessageKeys.ApartmentAlreadyOccupied, apartmentId)
    {
    }
}
