using MediatR;

namespace SiteManagement.Application.Residency.Commands.UpdateContactInfo;

/// <summary>Updates email + phone on an existing resident. Admin-only.</summary>
public sealed record UpdateContactInfoCommand(Guid ResidentId, string Email, string Phone) : IRequest;
