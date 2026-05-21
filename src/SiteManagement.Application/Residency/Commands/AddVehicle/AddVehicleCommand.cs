using SiteManagement.Application.Abstractions.Messaging;

namespace SiteManagement.Application.Residency.Commands.AddVehicle;

/// <summary>Registers a vehicle on an existing resident. Admin-only.</summary>
public sealed record AddVehicleCommand(Guid ResidentId, string Plate, string? Note) : ICommand;
