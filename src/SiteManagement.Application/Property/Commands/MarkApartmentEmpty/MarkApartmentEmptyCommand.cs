using SiteManagement.Application.Abstractions.Messaging;

namespace SiteManagement.Application.Property.Commands.MarkApartmentEmpty;

/// <summary>Flips the apartment's occupancy from Occupied to Empty. Admin-only.</summary>
public sealed record MarkApartmentEmptyCommand(Guid ApartmentId) : ICommand, IAdminRequest;
