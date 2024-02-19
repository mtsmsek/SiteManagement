using MediatR;
using SiteManagement.Application.CrossCuttingConcerns.Caching;
using SiteManagement.Domain.Constants.Buildings.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Features.Commands.Buildings.Blocks.CreateBlock
{
    public class CreateBlockCommand : IRequest<Guid>, ICacheRemoverRequest
    {
        private string _name;
        public string Name { get { return _name; } set { _name = value.ToUpper(); } } 

        public string? CacheKey => BlockMessages.CacheKeys.CacheKey;

        public bool BypassCache => false;
    }
}
