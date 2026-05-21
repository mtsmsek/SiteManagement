using SiteManagement.Application.Abstractions.Messaging;

namespace SiteManagement.Application.Property.Commands.AddBlock;

/// <summary>Adds a new block to an existing site. Admin-only command.</summary>
public sealed record AddBlockCommand(Guid SiteId, string Name) : ICommand<AddBlockResult>;

/// <summary>Result carrying the new block's identifier.</summary>
public sealed record AddBlockResult(Guid BlockId);
