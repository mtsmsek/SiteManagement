using SiteManagement.Domain.Shared.Exceptions;

namespace SiteManagement.Domain.Property.Exceptions;

/// <summary>
/// Thrown when a site name is empty / whitespace / longer than
/// <see cref="PropertyLimits.SiteNameMaxLength"/>.
/// </summary>
public sealed class InvalidSiteNameException : DomainException
{
    /// <summary>Creates the exception.</summary>
    public InvalidSiteNameException()
        : base(PropertyMessageKeys.SiteNameInvalid, PropertyLimits.SiteNameMaxLength)
    {
    }
}
