using SiteManagement.Domain.Shared.Exceptions;

namespace SiteManagement.Domain.Residency.Exceptions;

/// <summary>
/// Thrown when the free-form note attached to a vehicle exceeds
/// <see cref="ResidencyLimits.VehicleNoteMaxLength"/>.
/// </summary>
public sealed class VehicleNoteTooLongException : DomainException
{
    /// <summary>Creates the exception.</summary>
    public VehicleNoteTooLongException()
        : base(ResidencyMessageKeys.VehicleNoteTooLong, ResidencyLimits.VehicleNoteMaxLength) { }
}
