using SiteManagement.Application.Abstractions.Messaging;

namespace SiteManagement.Application.Tenancy.Commands.EndAssignment;

/// <summary>
/// Ends an apartment assignment on <paramref name="EndDate"/> (move-out). The
/// assignment raises <c>ResidentMovedOut</c>, which a Property-side handler
/// reacts to by marking the apartment empty again. Admin-only command.
/// </summary>
public sealed record EndAssignmentCommand(Guid AssignmentId, DateOnly EndDate) : ICommand;
