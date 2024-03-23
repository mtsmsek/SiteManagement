using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SiteManagement.Application.Security.Extensions;
using System.Security.Claims;

namespace SiteManagement.Api.WebApi.Controllers.Commons
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected Guid getUserIdFromRequest() //todo authentication behavior?
        {
            Guid userId = HttpContext.User.GetUserId();
            return userId;
        }
        
        protected IMediator? Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();
        private IMediator? _mediator;
    }
}
