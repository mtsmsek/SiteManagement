using SiteManagement.Domain.Shared.Exceptions;

namespace SiteManagement.Domain.Property.Exceptions;

/// <summary>
/// Thrown when an operation references a block id that does not belong to
/// the targeted site.
/// </summary>
public sealed class BlockNotFoundInSiteException : DomainException
{
    /// <summary>Creates the exception for the missing block id.</summary>
    public BlockNotFoundInSiteException(Guid blockId)
        : base(PropertyMessageKeys.BlockNotFoundInSite, blockId)
    {
    }
}
