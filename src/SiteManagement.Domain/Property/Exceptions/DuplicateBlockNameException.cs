using SiteManagement.Domain.Shared.Exceptions;

namespace SiteManagement.Domain.Property.Exceptions;

/// <summary>
/// Thrown when a site already contains a block with the supplied name.
/// </summary>
public sealed class DuplicateBlockNameException : DomainException
{
    /// <summary>Creates the exception describing the duplicated name.</summary>
    public DuplicateBlockNameException(string blockName)
        : base(PropertyMessageKeys.DuplicateBlockName, blockName)
    {
    }
}
