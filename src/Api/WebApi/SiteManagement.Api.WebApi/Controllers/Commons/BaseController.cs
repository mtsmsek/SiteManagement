using MediatR;
using Microsoft.AspNetCore.Mvc;
using SiteManagement.Application.Security.Extensions;

namespace SiteManagement.Api.WebApi.Controllers.Commons;

[Route("api/[controller]")]
[ApiController]
public class BaseController : ControllerBase
{
    protected Guid getUserIdFromRequest() //todo in authentication behavior?
    {
        Guid userId = HttpContext.User.GetUserId();
        return userId;
    }
    
    protected IMediator? Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();
    private IMediator? _mediator;
}
