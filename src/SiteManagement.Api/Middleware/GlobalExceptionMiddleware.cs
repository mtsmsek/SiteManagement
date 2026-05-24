using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using SiteManagement.Api.Configuration;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Application.Shared.Resources;
using SiteManagement.Application.Shared.Validation;
using AppException = SiteManagement.Application.Shared.Exceptions.ApplicationException;

namespace SiteManagement.Api.Middleware;

/// <summary>
/// Final stop for any exception thrown by the request pipeline. Renders
/// <see cref="AppException"/> subclasses as RFC 7807 ProblemDetails using the
/// status code carried by the exception; renders anything else as 500.
/// Domain exceptions never reach this point — the MediatR
/// <c>ExceptionTranslationBehavior</c> translates them earlier.
/// </summary>
/// <remarks>
/// Localization happens here for the payloads carried out of the Application
/// layer as stable keys: <see cref="AuthenticationException.MessageKey"/>,
/// <see cref="BusinessRuleViolationException.MessageKey"/>, and the
/// per-failure <see cref="ValidationFailureDetail.MessageKey"/> entries.
/// The Application layer stays free of any IStringLocalizer dependency.
/// </remarks>
public class GlobalExceptionMiddleware(
    RequestDelegate next,
    ILogger<GlobalExceptionMiddleware> logger,
    IStringLocalizer<ErrorMessages> errorLocalizer,
    IStringLocalizer<ValidationMessages> validationLocalizer)
{
    private const string ContentTypeProblemJson = "application/problem+json";
    private const string TypeUriPrefix = "https://httpstatuses.io/";
    private const string UnhandledTitle = "An unexpected error occurred.";
    private const string UnhandledDetail = "The server encountered an unexpected condition.";

    private static readonly Regex PlaceholderPattern = new(@"\{(?<name>[A-Za-z0-9_]+)\}", RegexOptions.Compiled);

    private readonly RequestDelegate _next = next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger = logger;
    private readonly IStringLocalizer<ErrorMessages> _errorLocalizer = errorLocalizer;
    private readonly IStringLocalizer<ValidationMessages> _validationLocalizer = validationLocalizer;

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
    private async Task WriteProblemDetailsAsync(HttpContext context, AppException ex)
    {
        var problem = new ProblemDetails
        {
            Status = ex.StatusCode,
            Title = ex.GetType().Name,
            Detail = LocalizeDetail(ex),
            Type = TypeUriPrefix + ex.StatusCode,
            Instance = context.Request.Path,
        };

        problem.Extensions[ApiConstants.ProblemDetailsTraceIdKey] = context.TraceIdentifier;

        switch (ex)
        {
            case ValidationException validation:
                problem.Extensions[ApiConstants.ProblemDetailsErrorsKey] = LocalizeValidationErrors(validation.Failures);
                break;
            case BusinessRuleViolationException business:
                problem.Extensions[ApiConstants.ProblemDetailsMessageKey] = business.MessageKey;
                break;
            case AuthenticationException authentication:
                problem.Extensions[ApiConstants.ProblemDetailsMessageKey] = authentication.MessageKey;
                break;
            case PaymentRejectedException payment:
                problem.Extensions[ApiConstants.ProblemDetailsMessageKey] = payment.MessageKey;
                break;
        }

        await WriteAsync(context, problem);
    }

    /// <summary>
    /// Picks the right localizer for the typed exception. Validator-side keys
    /// resolve against <see cref="ValidationMessages"/>. Business-rule
    /// violations were already localized — with their format arguments
    /// substituted — by the MediatR <c>ExceptionTranslationBehavior</c>, so we
    /// use that ready message rather than re-localizing the bare key here
    /// (which would lose the args and leak raw "{0}" placeholders). Auth keys
    /// carry no args, so they are localized here against
    /// <see cref="ErrorMessages"/>. A missing key falls back to the raw value
    /// (developer-friendly diagnostic).
    /// </summary>
    private string LocalizeDetail(AppException ex) => ex switch
    {
        ValidationException => _errorLocalizer[ErrorMessageKeys.ValidationFailed].Value,
        BusinessRuleViolationException business => business.Message,
        AuthenticationException auth => Localize(_errorLocalizer, auth.MessageKey),
        PaymentRejectedException payment => Localize(_errorLocalizer, payment.MessageKey),
        _ => ex.Message,
    };

    /// <summary>
    /// Translates each <see cref="ValidationFailureDetail.MessageKey"/> and
    /// expands FluentValidation placeholders (e.g. <c>{PropertyName}</c>,
    /// <c>{MaxLength}</c>) into the localized template.
    /// </summary>
    private Dictionary<string, string[]> LocalizeValidationErrors(
        IReadOnlyDictionary<string, ValidationFailureDetail[]> failures)
        => failures.ToDictionary(
            kv => kv.Key,
            kv => kv.Value.Select(f => ApplyPlaceholders(
                    Localize(_validationLocalizer, f.MessageKey),
                    f.Placeholders))
                .ToArray());

    /// <summary>
    /// Replaces every <c>{name}</c> token in <paramref name="template"/> with
    /// the matching value from <paramref name="placeholders"/>. Unknown
    /// tokens stay as-is so missing data is visible during development.
    /// </summary>
    private static string ApplyPlaceholders(string template, IReadOnlyDictionary<string, object> placeholders)
    {
        if (placeholders.Count == 0)
        {
            return template;
        }

        return PlaceholderPattern.Replace(template, match =>
        {
            var name = match.Groups["name"].Value;
            return placeholders.TryGetValue(name, out var value) ? value?.ToString() ?? string.Empty : match.Value;
        });
    }

    private static string Localize<T>(IStringLocalizer<T> localizer, string key)
    {
        var entry = localizer[key];
        return entry.ResourceNotFound ? key : entry.Value;
    }

    /// <summary>Generic 500 ProblemDetails body for anything that wasn't an application exception.</summary>
    private async Task WriteUnhandledAsync(HttpContext context)
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
