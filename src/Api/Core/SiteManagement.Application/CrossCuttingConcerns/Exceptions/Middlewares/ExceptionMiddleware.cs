using Microsoft.AspNetCore.Http;
using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.CrossCuttingConcerns.Exceptions.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly HttpExceptionHandler _httpExceptionHandler;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
        _httpExceptionHandler = new HttpExceptionHandler();
    }
    public virtual async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);

        }
        catch (Exception exception)
        {

            await HandleExceptionAsync(context.Response,exception);
        }
    }
    private Task HandleExceptionAsync(HttpResponse response, Exception exception)
    {
        response.ContentType = "application/json";
        _httpExceptionHandler.Response = response;
        return _httpExceptionHandler.HandleExceptionAsync(exception);
    }
}
