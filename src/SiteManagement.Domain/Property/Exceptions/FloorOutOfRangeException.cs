using SiteManagement.Domain.Shared.Exceptions;

namespace SiteManagement.Domain.Property.Exceptions;

/// <summary>
/// Thrown when a floor number falls outside
/// [<see cref="PropertyLimits.FloorMin"/>..<see cref="PropertyLimits.FloorMax"/>].
/// </summary>
public sealed class FloorOutOfRangeException : DomainException
{
    /// <summary>Creates the exception describing the offending value.</summary>
    public FloorOutOfRangeException(int value)
        : base(PropertyMessageKeys.FloorOutOfRange,
               value,
               PropertyLimits.FloorMin,
               PropertyLimits.FloorMax)
    {
    }
}
