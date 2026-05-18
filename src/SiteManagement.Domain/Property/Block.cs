using SiteManagement.Domain.Property.Exceptions;
using SiteManagement.Domain.Property.ValueObjects;
using SiteManagement.Domain.Shared;

namespace SiteManagement.Domain.Property;

/// <summary>
/// A named container of apartments inside a <see cref="Site"/>. Guards the
/// uniqueness of apartment numbers within itself and exposes its children
/// as a read-only collection so callers can't bypass invariants by poking
/// at the backing list.
/// </summary>
public sealed class Block : Entity<Guid>
{
    private readonly List<Apartment> _apartments = [];

    /// <summary>Display name (e.g. <c>"A"</c>, <c>"Block-7"</c>). Equality is case-insensitive.</summary>
    public BlockName Name { get; private set; }

    /// <summary>Read-only view over the apartments owned by this block.</summary>
    public IReadOnlyCollection<Apartment> Apartments => _apartments.AsReadOnly();

    // EF Core materialisation ctor.
    private Block()
    {
        Name = default!;
    }

    private Block(Guid id, BlockName name) : base(id)
    {
        Name = name;
    }

    /// <summary>Creates a brand-new empty block.</summary>
    public static Block Create(BlockName name) => new(Guid.NewGuid(), name);

    /// <summary>Adds an apartment, rejecting duplicate numbers.</summary>
    /// <exception cref="DuplicateApartmentNumberException">Thrown when an apartment with the same number already exists.</exception>
    public void AddApartment(Apartment apartment)
    {
        if (_apartments.Any(a => a.Number == apartment.Number))
        {
            throw new DuplicateApartmentNumberException(apartment.Number.Value);
        }

        _apartments.Add(apartment);
    }

    /// <summary>Removes the apartment with the given id.</summary>
    /// <exception cref="ApartmentNotFoundInBlockException">Thrown when no apartment with that id exists.</exception>
    public void RemoveApartment(Guid apartmentId)
    {
        var existing = _apartments.FirstOrDefault(a => a.Id == apartmentId)
            ?? throw new ApartmentNotFoundInBlockException(apartmentId);

        _apartments.Remove(existing);
    }

    /// <summary>Renames the block.</summary>
    public void Rename(BlockName newName)
    {
        Name = newName;
    }

    /// <summary>Locates an apartment by id, or throws.</summary>
    /// <exception cref="ApartmentNotFoundInBlockException">Thrown when the id is unknown.</exception>
    public Apartment GetApartment(Guid apartmentId)
        => _apartments.FirstOrDefault(a => a.Id == apartmentId)
           ?? throw new ApartmentNotFoundInBlockException(apartmentId);
}
