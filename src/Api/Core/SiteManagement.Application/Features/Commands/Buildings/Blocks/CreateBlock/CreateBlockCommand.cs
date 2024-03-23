using MediatR;
using SiteManagement.Application.CrossCuttingConcerns.Caching;
using SiteManagement.Application.Pipelines.Authorization;
using SiteManagement.Domain.Constants.Buildings.Blocks;
using SiteManagement.Domain.Constants.Security;
using static SiteManagement.Domain.Constants.Security.UsersOperationClaims;
namespace SiteManagement.Application.Features.Commands.Buildings.Blocks.CreateBlock
{
    public class CreateBlockCommand : IRequest<Guid>, ICacheRemoverRequest , ISecuredRequest
    {
        private string _name;
        public string Name { get { return _name; } set { _name = value.ToUpper(); } } 

        public string? CacheKey => BlockMessages.CacheKeys.CacheKey;

        public bool BypassCache => false;

        public string[] Roles => new[] { Admin, Add };
    }
}
