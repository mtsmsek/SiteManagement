namespace SiteManagement.Application.Shared.Exceptions;

/// <summary>
/// Thrown when the caller is authenticated but not allowed to perform the
/// requested action (e.g. a Resident trying to pay another resident's invoice).
/// Renders as HTTP 403.
/// </summary>
public sealed class UnauthorizedActionException : ApplicationException
{
    /// <summary>Creates a 403 with a localized message.</summary>
    public UnauthorizedActionException(string message) : base(message, HttpStatus.Forbidden) { }
}
