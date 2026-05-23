using SiteManagement.Domain.Property.Exceptions;
using SiteManagement.Domain.Property.ValueObjects;
using SiteManagement.Domain.Shared;

namespace SiteManagement.Domain.Property;

/// <summary>
/// Aggregate root for the Property bounded context. A site owns blocks
/// (which in turn own apartments). The site is the only entity the outside
/// world saves and loads as a unit; inner blocks and apartments are
/// modified through methods on the root or on the root's children, but the
/// persistence boundary stays here.
/// </summary>
public sealed class Site : AggregateRoot<Guid>, ISoftDeletable
{
    private readonly List<Block> _blocks = [];

    /// <inheritdoc />
    public bool IsDeleted { get; private set; }

    /// <inheritdoc />
    public DateTime? DeletedOnUtc { get; private set; }

    /// <summary>Display name of the site (e.g. <c>"Lavender Heights"</c>).</summary>
    public string Name { get; private set; }

    /// <summary>Postal address of the site, free-form for now.</summary>
    public string Address { get; private set; }

    /// <summary>Read-only view over the blocks owned by this site.</summary>
    public IReadOnlyCollection<Block> Blocks => _blocks.AsReadOnly();

    // EF Core materialisation ctor.
    private Site()
    {
        Name = string.Empty;
        Address = string.Empty;
    }

    private Site(Guid id, string name, string address) : base(id)
    {
        Name = name;
        Address = address;
    }

    /// <summary>Creates a brand-new site with no blocks.</summary>
    /// <exception cref="InvalidSiteNameException">Thrown when the name is empty or longer than the allowed maximum.</exception>
    public static Site Create(string name, string address)
    {
        var normalisedName = NormaliseName(name);
        var normalisedAddress = (address ?? string.Empty).Trim();
        return new Site(Guid.NewGuid(), normalisedName, normalisedAddress);
    }

    /// <summary>Adds a block, rejecting names that already exist in this site.</summary>
    /// <exception cref="DuplicateBlockNameException">Thrown when a block with the same name (case-insensitive) already exists.</exception>
    public Block AddBlock(BlockName name)
    {
        if (_blocks.Any(b => b.Name == name))
        {
            throw new DuplicateBlockNameException(name.Value);
        }

        var block = Block.Create(name);
        _blocks.Add(block);
        return block;
    }

    /// <summary>Removes the block with the given id.</summary>
    /// <exception cref="BlockNotFoundInSiteException">Thrown when no block with that id exists.</exception>
    public void RemoveBlock(Guid blockId)
    {
        var existing = _blocks.FirstOrDefault(b => b.Id == blockId)
            ?? throw new BlockNotFoundInSiteException(blockId);

        _blocks.Remove(existing);
    }

    /// <summary>Renames the site.</summary>
    /// <exception cref="InvalidSiteNameException">Thrown when the new name is empty or too long.</exception>
    public void Rename(string newName)
    {
        Name = NormaliseName(newName);
    }

    /// <summary>Locates a block by id, or throws.</summary>
    /// <exception cref="BlockNotFoundInSiteException">Thrown when the id is unknown.</exception>
    public Block GetBlock(Guid blockId)
        => _blocks.FirstOrDefault(b => b.Id == blockId)
           ?? throw new BlockNotFoundInSiteException(blockId);

    /// <summary>Validates and trims a site name.</summary>
    /// <exception cref="InvalidSiteNameException">Thrown when the name is empty or longer than the allowed maximum.</exception>
    private static string NormaliseName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidSiteNameException();
        }

        var trimmed = name.Trim();
        if (trimmed.Length > PropertyLimits.SiteNameMaxLength)
        {
            throw new InvalidSiteNameException();
        }

        return trimmed;
    }

    /// <summary>
    /// Archives the site (soft delete). The aggregate's blocks and apartments
    /// are reached only through the site, so hiding the archived root hides the
    /// whole tree — no per-child flag needed. Idempotent.
    /// </summary>
    public void Archive(DateTime whenUtc)
    {
        if (IsDeleted)
        {
            return;
        }

        IsDeleted = true;
        DeletedOnUtc = whenUtc;
    }
}
