using CO.Application.Contracts.Interfaces.Common;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace CO.Application.Behaviours;

public class CachingBehavior<TRequest, TResponse>
    (ICacheService cache, ILogger<CachingBehavior<TRequest, TResponse>> logger) 
    
    : IPipelineBehavior<TRequest, TResponse> where TRequest 
    : IRequest<TResponse>, ICachableRequest
     
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        if (!request.ShouldCache)
            return await next(ct);

        string cacheKey;

        if (request.IsVersioned)
        {
            // Resolve or create version token
            var version = await cache.GetAsync<string>(request.CacheKeyPrefix, ct);
            if (version is null)
            {
                version = GenerateVersion();
                await cache.SetAsync(
                    request.CacheKeyPrefix,
                    version,
                    TimeSpan.FromDays(1), // version itself lives long
                    ct);
            }

            cacheKey = $"{request.CacheKeyPrefix}:{version}:{request.CacheKeySuffix}";
        }
        else
        {
            cacheKey = $"{request.CacheKeyPrefix}:{request.CacheKeySuffix}";
        }

        logger.LogInformation("Cache lookup: {CacheKey}", cacheKey);

        var cached = await cache.GetAsync<TResponse>(cacheKey, ct);
        if (cached is not null)
        {
            logger.LogInformation("Cache hit: {CacheKey}", cacheKey);
            return cached;
        }

        logger.LogInformation("Cache miss: {CacheKey}", cacheKey);
        var response = await next(ct);

        if (response is not null)
            await cache.SetAsync(cacheKey, response, request.Expiration ?? TimeSpan.FromMinutes(30), ct);

        return response;
    }

    private static string GenerateVersion()
        => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
}
