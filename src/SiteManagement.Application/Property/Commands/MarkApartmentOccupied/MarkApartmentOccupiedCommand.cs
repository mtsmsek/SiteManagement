using MediatR;

namespace SiteManagement.Application.Property.Commands.MarkApartmentOccupied;

/// <summary>Flips the apartment's occupancy from Empty to Occupied. Admin-only.</summary>
public sealed record MarkApartmentOccupiedCommand(Guid ApartmentId) : IRequest;
