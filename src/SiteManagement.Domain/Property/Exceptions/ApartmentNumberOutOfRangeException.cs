using SiteManagement.Domain.Shared.Exceptions;

namespace SiteManagement.Domain.Property.Exceptions;

/// <summary>
/// Thrown when an apartment number falls outside
/// [<see cref="PropertyLimits.ApartmentNumberMin"/>..<see cref="PropertyLimits.ApartmentNumberMax"/>].
/// </summary>
public sealed class ApartmentNumberOutOfRangeException : DomainException
{
    /// <summary>Creates the exception describing the offending value.</summary>
    public ApartmentNumberOutOfRangeException(int value)
        : base(PropertyMessageKeys.ApartmentNumberOutOfRange,
               value,
               PropertyLimits.ApartmentNumberMin,
               PropertyLimits.ApartmentNumberMax)
    {
    }
}
