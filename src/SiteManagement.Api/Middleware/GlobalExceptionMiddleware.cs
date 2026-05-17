using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using SiteManagement.Api.Configuration;
using SiteManagement.Application.Shared.Exceptions;
using AppException = SiteManagement.Application.Shared.Exceptions.ApplicationException;

namespace SiteManagement.Api.Middleware;

/// <summary>
/// Final stop for any exception thrown by the request pipeline. Renders
/// <see cref="AppException"/> subclasses as RFC 7807 ProblemDetails using the
/// status code carried by the exception; renders anything else as 500.
/// Domain exceptions never reach this point — the MediatR
/// <c>ExceptionTranslationBehavior</c> translates them earlier.
/// </summary>
public class GlobalExceptionMiddleware(
    RequestDelegate next,
    ILogger<GlobalExceptionMiddleware> logger)
{
    private const string ContentTypeProblemJson = "application/problem+json";
    private const string TypeUriPrefix = "https://httpstatuses.io/";
    private const string UnhandledTitle = "An unexpected error occurred.";
    private const string UnhandledDetail = "The server encountered an unexpected condition.";

    private readonly RequestDelegate _next = next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger = logger;

    /// <summary>Wraps the next request delegate in a top-level try/catch.</summary>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (AppException appEx)
        {
            _logger.LogInformation(
                appEx,
                "Application exception {ExceptionType} ({StatusCode}) at {Path}",
                appEx.GetType().Name,
                appEx.StatusCode,
                context.Request.Path);

            await WriteProblemDetailsAsync(context, appEx);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception at {Path}", context.Request.Path);
            await WriteUnhandledAsync(context);
        }
    }

    /// <summary>Maps a typed application exception to its ProblemDetails representation.</summary>
    private static async Task WriteProblemDetailsAsync(HttpContext context, AppException ex)
    {
        var problem = new ProblemDetails
        {
            Status = ex.StatusCode,
            Title = ex.GetType().Name,
            Detail = ex.Message,
            Type = TypeUriPrefix + ex.StatusCode,
            Instance = context.Request.Path,
        };

        problem.Extensions[ApiConstants.ProblemDetailsTraceIdKey] = context.TraceIdentifier;

        switch (ex)
        {
            case ValidationException validation:
                problem.Extensions[ApiConstants.ProblemDetailsErrorsKey] = validation.Errors;
                break;
            case BusinessRuleViolationException business:
                problem.Extensions[ApiConstants.ProblemDetailsMessageKey] = business.MessageKey;
                break;
        }

        await WriteAsync(context, problem);
    }

    /// <summary>Generic 500 ProblemDetails body for anything that wasn't an application exception.</summary>
    private static async Task WriteUnhandledAsync(HttpContext context)
    {
        var problem = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = UnhandledTitle,
            Detail = UnhandledDetail,
            Type = TypeUriPrefix + StatusCodes.Status500InternalServerError,
            Instance = context.Request.Path,
        };
        problem.Extensions[ApiConstants.ProblemDetailsTraceIdKey] = context.TraceIdentifier;

        await WriteAsync(context, problem);
    }

    /// <summary>Serializes the body, setting status code + content type once.</summary>
    private static async Task WriteAsync(HttpContext context, ProblemDetails problem)
    {
        if (context.Response.HasStarted)
        {
            return;
        }

        context.Response.Clear();
        context.Response.StatusCode = problem.Status ?? StatusCodes.Status500InternalServerError;
        context.Response.ContentType = ContentTypeProblemJson;

        var json = JsonSerializer.Serialize(problem, JsonOptions.Default);
        await context.Response.WriteAsync(json);
    }

    /// <summary>Shared JsonSerializerOptions for problem payloads.</summary>
    private static class JsonOptions
    {
        public static readonly JsonSerializerOptions Default = new(JsonSerializerDefaults.Web);
    }
}
