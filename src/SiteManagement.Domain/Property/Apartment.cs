using SiteManagement.Domain.Property.Exceptions;
using SiteManagement.Domain.Property.ValueObjects;
using SiteManagement.Domain.Shared;

namespace SiteManagement.Domain.Property;

/// <summary>
/// A single apartment within a block. Lives inside the <see cref="Site"/>
/// aggregate (Site is the root; Block and Apartment are inner entities),
/// owns its own occupancy state machine, and rejects illegal transitions
/// at the entity boundary instead of relying on callers to check first.
/// </summary>
public sealed class Apartment : Entity<Guid>
{
    /// <summary>Apartment number within the owning block (unique per block).</summary>
    public ApartmentNumber Number { get; private set; }

    /// <summary>Floor (signed: negative = basement).</summary>
    public Floor Floor { get; private set; }

    /// <summary>Layout (rooms + living rooms).</summary>
    public ApartmentType Type { get; private set; }

    /// <summary>Current occupancy status; mutated only via <see cref="MarkAsOccupied"/> / <see cref="MarkAsEmpty"/>.</summary>
    public OccupancyStatus Status { get; private set; }

    // EF Core materialisation ctor; never used by production code.
    private Apartment()
    {
        Number = default!;
        Floor = default!;
        Type = default!;
    }

    private Apartment(Guid id, ApartmentNumber number, Floor floor, ApartmentType type)
        : base(id)
    {
        Number = number;
        Floor = floor;
        Type = type;
        Status = OccupancyStatus.Empty;
    }

    /// <summary>Canonical factory for a brand-new apartment in the <see cref="OccupancyStatus.Empty"/> state.</summary>
    public static Apartment Create(ApartmentNumber number, Floor floor, ApartmentType type)
        => new(Guid.NewGuid(), number, floor, type);

    /// <summary>Transitions to <see cref="OccupancyStatus.Occupied"/>.</summary>
    /// <exception cref="ApartmentAlreadyOccupiedException">Thrown when already occupied.</exception>
    public void MarkAsOccupied()
    {
        if (Status == OccupancyStatus.Occupied)
        {
            throw new ApartmentAlreadyOccupiedException(Id);
        }

        Status = OccupancyStatus.Occupied;
    }

    /// <summary>Transitions to <see cref="OccupancyStatus.Empty"/>.</summary>
    /// <exception cref="ApartmentNotOccupiedException">Thrown when already empty.</exception>
    public void MarkAsEmpty()
    {
        if (Status == OccupancyStatus.Empty)
        {
            throw new ApartmentNotOccupiedException(Id);
        }

        Status = OccupancyStatus.Empty;
    }

    /// <summary>Replaces the apartment layout (e.g. after a remodel).</summary>
    public void ChangeType(ApartmentType newType)
    {
        Type = newType;
    }
}
