using MediatR;

namespace SiteManagement.Application.Property.Commands.AddApartment;

/// <summary>
/// Adds an apartment to the supplied block. <c>BlockId</c> is globally
/// unique, so the parent <c>SiteId</c> is resolved server-side and is not
/// part of the contract.
/// </summary>
public sealed record AddApartmentCommand(
    Guid BlockId,
    int Number,
    int Floor,
    string Type) : IRequest<AddApartmentResult>;

/// <summary>Result carrying the new apartment's identifier.</summary>
public sealed record AddApartmentResult(Guid ApartmentId);
