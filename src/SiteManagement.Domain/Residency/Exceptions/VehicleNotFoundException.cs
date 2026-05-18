using SiteManagement.Domain.Shared.Exceptions;

namespace SiteManagement.Domain.Residency.Exceptions;

/// <summary>
/// Thrown when a remove/update operation references a plate that is not
/// registered on the resident.
/// </summary>
public sealed class VehicleNotFoundException : DomainException
{
    /// <summary>Creates the exception for the missing plate.</summary>
    public VehicleNotFoundException(string plate)
        : base(ResidencyMessageKeys.VehicleNotFound, plate) { }
}
