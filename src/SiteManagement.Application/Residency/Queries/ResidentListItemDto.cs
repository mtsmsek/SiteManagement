namespace SiteManagement.Application.Residency.Queries;

/// <summary>
/// Row for the admin "all residents" list page. Phones / email shown as
/// already-normalised strings; the vehicles list is summarised by count
/// here and expanded only when the detail projection is fetched.
/// </summary>
public sealed record ResidentListItemDto(
    Guid Id,
    string TcNo,
    string FirstName,
    string LastName,
    string Email,
    string Phone,
    int VehicleCount);
