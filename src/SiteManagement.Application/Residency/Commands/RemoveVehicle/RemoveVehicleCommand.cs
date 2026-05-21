using SiteManagement.Application.Abstractions.Messaging;

namespace SiteManagement.Application.Residency.Commands.RemoveVehicle;

/// <summary>Unregisters a vehicle from a resident by plate. Admin-only.</summary>
public sealed record RemoveVehicleCommand(Guid ResidentId, string Plate) : ICommand;
