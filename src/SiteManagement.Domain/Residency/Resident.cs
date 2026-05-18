using SiteManagement.Domain.Residency.Exceptions;
using SiteManagement.Domain.Residency.ValueObjects;
using SiteManagement.Domain.Shared;

namespace SiteManagement.Domain.Residency;

/// <summary>
/// Aggregate root for the Residency bounded context. A resident is the
/// person behind an apartment assignment, the dues / invoice records, and
/// the messaging conversations. From W2 onward the
/// <c>SiteManagement.Infrastructure.Identity.AppUser</c> links to a
/// Resident via this aggregate's id; the AppUser stays a persistence
/// concern, while the business identity lives here.
/// </summary>
public sealed class Resident : AggregateRoot<Guid>
{
    private readonly List<VehicleInfo> _vehicles = [];

    /// <summary>Turkish citizenship number; immutable.</summary>
    public TcNo TcNo { get; private set; }

    /// <summary>Display name; mutable via <see cref="ChangeFullName"/>.</summary>
    public FullName FullName { get; private set; }

    /// <summary>Contact email; mutable via <see cref="UpdateContactInfo"/>.</summary>
    public Email Email { get; private set; }

    /// <summary>Contact phone; mutable via <see cref="UpdateContactInfo"/>.</summary>
    public PhoneNumber Phone { get; private set; }

    /// <summary>Read-only view over the registered vehicles.</summary>
    public IReadOnlyCollection<VehicleInfo> Vehicles => _vehicles.AsReadOnly();

    // EF Core materialisation ctor.
    private Resident()
    {
        TcNo = default!;
        FullName = default!;
        Email = default!;
        Phone = default!;
    }

    private Resident(Guid id, TcNo tcNo, FullName fullName, Email email, PhoneNumber phone) : base(id)
    {
        TcNo = tcNo;
        FullName = fullName;
        Email = email;
        Phone = phone;
    }

    /// <summary>Canonical factory for a brand-new resident with no vehicles.</summary>
    public static Resident Create(TcNo tcNo, FullName fullName, Email email, PhoneNumber phone)
        => new(Guid.NewGuid(), tcNo, fullName, email, phone);

    /// <summary>Replaces both contact channels in one transactional move.</summary>
    public void UpdateContactInfo(Email email, PhoneNumber phone)
    {
        Email = email;
        Phone = phone;
    }

    /// <summary>Renames the resident.</summary>
    public void ChangeFullName(FullName newName)
    {
        FullName = newName;
    }

    /// <summary>Registers a vehicle, rejecting duplicate plates.</summary>
    /// <exception cref="DuplicateVehiclePlateException">Thrown when the plate is already registered on this resident.</exception>
    public void AddVehicle(VehicleInfo vehicle)
    {
        if (_vehicles.Any(v => v.Plate == vehicle.Plate))
        {
            throw new DuplicateVehiclePlateException(vehicle.Plate.Value);
        }

        _vehicles.Add(vehicle);
    }

    /// <summary>Removes the vehicle registered under the given plate.</summary>
    /// <exception cref="VehicleNotFoundException">Thrown when the plate is not registered on this resident.</exception>
    public void RemoveVehicle(PlateNumber plate)
    {
        var existing = _vehicles.FirstOrDefault(v => v.Plate == plate)
            ?? throw new VehicleNotFoundException(plate.Value);

        _vehicles.Remove(existing);
    }
}
