﻿using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SiteManagement.Application.CrossCuttingConcerns.Caching
{
    public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>, ICachableRequest
    {
        private readonly CacheSettings _cacheSettings;
        private readonly IDistributedCache _cache;
        private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;

        public CachingBehavior(IDistributedCache cache, ILogger<CachingBehavior<TRequest, TResponse>> logger, IConfiguration configuration)
        {
            _cacheSettings = configuration.GetSection("CacheSettings").Get<CacheSettings>() ?? throw new InvalidOperationException();
            _cache = cache;
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (request.BypassCache)
                return await next();

            TResponse response;
            byte[]? cachedResponse = await _cache.GetAsync(request.CacheKey, cancellationToken);

            if(cachedResponse != null)
            {
                response = JsonSerializer.Deserialize<TResponse>(Encoding.UTF8.GetString(cachedResponse))!;
                _logger.LogInformation($"Fetched from cache ->  {request.CacheKey}");

            }
            else
            {
                response = await getResponseAndAddToCache(request,next,cancellationToken);
            }


            return response;
        }

        private async Task<TResponse?> getResponseAndAddToCache(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            TResponse response = await next();
            TimeSpan slidingExpiration = request.SlidingExpiration ?? TimeSpan.FromDays(_cacheSettings.SlidingExpiration);
            
            DistributedCacheEntryOptions cacheEntryOptions = new() { SlidingExpiration = slidingExpiration };

            byte[] serializedData = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response));

            await _cache.SetAsync(request.CacheKey, serializedData, cancellationToken);
            _logger.LogInformation($"Added to cache {request.CacheKey}");


            return response;
        }

        
    }
}
