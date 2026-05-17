namespace SiteManagement.Application.Shared.Exceptions;

/// <summary>
/// Base class for every exception that may propagate out of the Application
/// layer. The Api layer catches these in <c>GlobalExceptionMiddleware</c> and
/// renders them as RFC 7807 ProblemDetails responses.
/// Domain exceptions never reach the Api directly — they are translated to
/// ApplicationException subclasses by <c>ExceptionTranslationBehavior</c>.
/// </summary>
public abstract class ApplicationException : Exception
{
    /// <summary>
    /// Creates an application exception with a (localized) message and the
    /// HTTP status code the middleware should emit.
    /// </summary>
    /// <param name="message">Already-localized message returned to the caller.</param>
    /// <param name="statusCode">HTTP status code emitted by the middleware.</param>
    /// <param name="inner">Underlying cause, typically a domain exception.</param>
    protected ApplicationException(string message, int statusCode, Exception? inner = null)
        : base(message, inner)
    {
        StatusCode = statusCode;
    }

    /// <summary>HTTP status code that should be returned to the client.</summary>
    public int StatusCode { get; }
}
