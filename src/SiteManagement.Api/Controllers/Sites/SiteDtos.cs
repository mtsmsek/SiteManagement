namespace SiteManagement.Api.Controllers.Sites;

/// <summary>Request body for <c>POST /api/sites</c>.</summary>
public sealed record CreateSiteRequest(string Name, string Address);

/// <summary>Response body for <c>POST /api/sites</c>.</summary>
public sealed record CreateSiteResponse(Guid SiteId);

/// <summary>Request body for <c>POST /api/sites/{siteId}/blocks</c>.</summary>
public sealed record AddBlockRequest(string Name);

/// <summary>Response body for <c>POST /api/sites/{siteId}/blocks</c>.</summary>
public sealed record AddBlockResponse(Guid BlockId);

/// <summary>Request body for <c>POST /api/blocks/{blockId}/apartments</c>.</summary>
public sealed record AddApartmentRequest(int Number, int Floor, string Type);

/// <summary>Response body for <c>POST /api/blocks/{blockId}/apartments</c>.</summary>
public sealed record AddApartmentResponse(Guid ApartmentId);
