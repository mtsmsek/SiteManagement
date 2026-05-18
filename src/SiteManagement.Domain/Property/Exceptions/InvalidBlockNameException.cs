using SiteManagement.Domain.Shared.Exceptions;

namespace SiteManagement.Domain.Property.Exceptions;

/// <summary>
/// Thrown when a block name is empty / whitespace / longer than
/// <see cref="PropertyLimits.BlockNameMaxLength"/>.
/// </summary>
public sealed class InvalidBlockNameException : DomainException
{
    /// <summary>Creates the exception.</summary>
    public InvalidBlockNameException()
        : base(PropertyMessageKeys.BlockNameInvalid, PropertyLimits.BlockNameMaxLength)
    {
    }
}
