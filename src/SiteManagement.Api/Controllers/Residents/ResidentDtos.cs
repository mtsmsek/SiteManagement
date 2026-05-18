namespace SiteManagement.Api.Controllers.Residents;

/// <summary>Request body for <c>POST /api/residents</c>.</summary>
public sealed record RegisterResidentRequest(
    string TcNo,
    string FirstName,
    string LastName,
    string Email,
    string Phone);

/// <summary>Response body for <c>POST /api/residents</c>.</summary>
public sealed record RegisterResidentResponse(Guid ResidentId);

/// <summary>Request body for <c>PUT /api/residents/{residentId}/contact</c>.</summary>
public sealed record UpdateContactRequest(string Email, string Phone);

/// <summary>Request body for <c>POST /api/residents/{residentId}/vehicles</c>.</summary>
public sealed record AddVehicleRequest(string Plate, string? Note);
