using CO.Application.Contracts.Interfaces.Common;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace CO.Application.Behaviours;

public class CacheInvalidationBehavior<TRequest, TResponse>
    (ICacheService cache, ILogger<CacheInvalidationBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, ICacheInvalidatorRequest
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        var response = await next(ct);

        // Direct key deletions (entity cache)
        foreach (var key in request.CacheKeysToInvalidate)
        {
            logger.LogInformation("Invalidating cache key: {Key}", key);
            await cache.RemoveAsync(key, ct);
        }

        // Version bumps (list cache — orphans all filter variants)
        foreach (var versionKey in request.CacheVersionKeysToInvalidate)
        {
            var newVersion = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
            logger.LogInformation("Bumping cache version: {VersionKey} → {Version}", versionKey, newVersion);
            await cache.SetAsync(versionKey, newVersion, TimeSpan.FromDays(1), ct);
        }

        return response;
    }
}
