using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.CrossCuttingConcerns.Caching
{
    public class CacheRemovingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>, ICacheRemoverRequest
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<CacheRemovingBehavior<TRequest, TResponse>> _logger;

        public CacheRemovingBehavior(IDistributedCache cache, ILogger<CacheRemovingBehavior<TRequest, TResponse>> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if(request.BypassCache)
            {
                return await next();
            }

            TResponse response = await next();

            if(request.CacheKey is not null)
            {
                await _cache.RemoveAsync(request.CacheKey, cancellationToken);
                _logger.LogInformation($"Cache removed -> {request.CacheKey}");
             
            }

            return response;

        }
    }
}
