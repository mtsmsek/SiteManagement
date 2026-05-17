using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace SiteManagement.Application.Behaviors;

/// <summary>
/// Logs every command/query that flows through MediatR with structured
/// properties: request type, elapsed milliseconds, and success/failure.
/// </summary>
public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger = logger;

    /// <inheritdoc />
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var stopwatch = Stopwatch.StartNew();

        _logger.LogInformation("Handling {RequestName}", requestName);
        try
        {
            var response = await next();
            stopwatch.Stop();
            _logger.LogInformation(
                "Handled {RequestName} in {ElapsedMs} ms",
                requestName,
                stopwatch.ElapsedMilliseconds);
            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogWarning(
                ex,
                "Handler {RequestName} threw {ExceptionType} after {ElapsedMs} ms",
                requestName,
                ex.GetType().Name,
                stopwatch.ElapsedMilliseconds);
            throw;
        }
    }
}
