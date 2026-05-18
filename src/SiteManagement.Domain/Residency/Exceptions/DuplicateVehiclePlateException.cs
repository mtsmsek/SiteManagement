using SiteManagement.Domain.Shared.Exceptions;

namespace SiteManagement.Domain.Residency.Exceptions;

/// <summary>
/// Thrown when a vehicle with the same plate number is already registered
/// on the resident.
/// </summary>
public sealed class DuplicateVehiclePlateException : DomainException
{
    /// <summary>Creates the exception describing the duplicated plate.</summary>
    public DuplicateVehiclePlateException(string plate)
        : base(ResidencyMessageKeys.DuplicateVehiclePlate, plate) { }
}
