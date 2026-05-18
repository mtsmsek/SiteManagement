using SiteManagement.Domain.Shared.Exceptions;

namespace SiteManagement.Domain.Property.Exceptions;

/// <summary>
/// Thrown when an operation references an apartment id that does not belong
/// to the targeted block.
/// </summary>
public sealed class ApartmentNotFoundInBlockException : DomainException
{
    /// <summary>Creates the exception for the missing apartment id.</summary>
    public ApartmentNotFoundInBlockException(Guid apartmentId)
        : base(PropertyMessageKeys.ApartmentNotFoundInBlock, apartmentId)
    {
    }
}
