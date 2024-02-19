using Microsoft.AspNetCore.Builder;
using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Middlewares;

namespace SiteManagement.Application.CrossCuttingConcerns.Exceptions.Extensions;

public static class ExceptionMiddlewareExtensions
{
    public static void ConfigureCustomExcepitonMiddleware(this IApplicationBuilder app )
        => app.UseMiddleware<ExceptionMiddleware>();
}
