using SiteManagement.Application.Abstractions.Messaging;

namespace SiteManagement.Application.Residency.Commands.RegisterResident;

/// <summary>
/// Creates a new Resident aggregate AND a matching <c>AppUser</c> in the
/// Resident role, linked by <c>AppUser.ResidentId</c>. Both writes commit
/// inside a single database transaction; the generated password is
/// emailed to the resident. Admin-only command.
/// </summary>
public sealed record RegisterResidentCommand(
    string TcNo,
    string FirstName,
    string LastName,
    string Email,
    string Phone) : ICommand<RegisterResidentResult>;

/// <summary>Result carrying the new resident's identifier.</summary>
public sealed record RegisterResidentResult(Guid ResidentId);
