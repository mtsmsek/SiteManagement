using MediatR;
using Microsoft.IdentityModel.Tokens;
using SiteManagement.Application.CrossCuttingConcerns.Caching;
using SiteManagement.Domain.Constants.Buildings.Blocks;
using static SiteManagement.Domain.Constants.Security.UsersOperationClaims;

namespace SiteManagement.Application.Features.Commands.Buildings.Blocks.CreateBlock;

public class CreateBlockCommand : IRequest<Guid>, ICacheRemoverRequest //, ISecuredRequest
{
    private string _name;
    public string Name
    {
        get { return _name; }
        set { _name = string.IsNullOrWhiteSpace(value) ? string.Empty : value.ToUpper(); }
    }

    public string? CacheKey => BlockMessages.CacheKeys.CacheKey;

    public bool BypassCache => false;

    public string[] Roles => new[] { Admin, Add };
}
