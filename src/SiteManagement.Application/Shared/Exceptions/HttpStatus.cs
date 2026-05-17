namespace SiteManagement.Application.Shared.Exceptions;

/// <summary>
/// HTTP status codes used by <see cref="ApplicationException"/> subclasses.
/// Defined here so the Application layer never depends on
/// <c>Microsoft.AspNetCore.Http</c> just to know that 404 means Not Found.
/// </summary>
public static class HttpStatus
{
    /// <summary>400 — request failed surface-level validation.</summary>
    public const int BadRequest = 400;

    /// <summary>401 — credentials missing/invalid or token expired.</summary>
    public const int Unauthorized = 401;

    /// <summary>403 — authenticated but not allowed.</summary>
    public const int Forbidden = 403;

    /// <summary>404 — referenced aggregate does not exist.</summary>
    public const int NotFound = 404;

    /// <summary>409 — request makes sense but violates a business rule.</summary>
    public const int Conflict = 409;
}
