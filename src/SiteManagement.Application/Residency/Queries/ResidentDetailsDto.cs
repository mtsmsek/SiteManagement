namespace SiteManagement.Application.Residency.Queries;

/// <summary>Resident detail projection used by the admin detail page.</summary>
public sealed record ResidentDetailsDto(
    Guid Id,
    string TcNo,
    string FirstName,
    string LastName,
    string Email,
    string Phone,
    IReadOnlyList<VehicleDto> Vehicles);

/// <summary>One vehicle row inside a <see cref="ResidentDetailsDto"/>.</summary>
public sealed record VehicleDto(string Plate, string? Note);
