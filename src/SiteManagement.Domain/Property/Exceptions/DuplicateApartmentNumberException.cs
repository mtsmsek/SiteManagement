using SiteManagement.Domain.Shared.Exceptions;

namespace SiteManagement.Domain.Property.Exceptions;

/// <summary>
/// Thrown when an apartment with the given number is already present in the
/// owning block.
/// </summary>
public sealed class DuplicateApartmentNumberException : DomainException
{
    /// <summary>Creates the exception describing the duplicate apartment number.</summary>
    public DuplicateApartmentNumberException(int number)
        : base(PropertyMessageKeys.DuplicateApartmentNumber, number)
    {
    }
}
