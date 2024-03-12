

using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;

namespace SiteManagement.Application.CrossCuttingConcerns.Exceptions.Handlers;

public abstract class ExceptionHandler
{
    public virtual Task HandleExceptionAsync(Exception exception) =>
        exception switch
        {
            BusinessException businessException => HandleException(businessException),
            ValidationException validationException => HandleException(exception),
            _ => HandleException(exception)
        };
    protected abstract Task HandleException(BusinessException businessException);
    protected abstract Task HandleException(ValidationException validationException);
    protected abstract Task HandleException(Exception exception);
}
