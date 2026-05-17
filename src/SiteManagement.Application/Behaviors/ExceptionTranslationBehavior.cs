using MediatR;
using Microsoft.Extensions.Localization;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Application.Shared.Resources;
using SiteManagement.Domain.Shared.Exceptions;
using AppException = SiteManagement.Application.Shared.Exceptions.ApplicationException;

namespace SiteManagement.Application.Behaviors;

/// <summary>
/// Central translation point for the Domain → Application boundary. Catches
/// any <see cref="DomainException"/> thrown by a handler, looks up the
/// localized message from <see cref="ErrorMessages"/>, and re-throws as a
/// <see cref="BusinessRuleViolationException"/> so the Api layer only ever
/// sees <see cref="AppException"/>.
/// </summary>
/// <remarks>
/// Handlers never wrap themselves in try/catch — this pipeline does it once.
/// </remarks>
public class ExceptionTranslationBehavior<TRequest, TResponse>(
    IStringLocalizer<ErrorMessages> localizer)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IStringLocalizer<ErrorMessages> _localizer = localizer;

    /// <inheritdoc />
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (DomainException ex)
        {
            var args = ex.MessageArgs.ToArray();
            var localized = args.Length == 0
                ? _localizer[ex.MessageKey].Value
                : _localizer[ex.MessageKey, args].Value;

            throw new BusinessRuleViolationException(localized, ex.MessageKey, ex);
        }
    }
}
