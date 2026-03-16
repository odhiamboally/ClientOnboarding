using CO.Application.Contracts.Interfaces.Common;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace CO.Infrastructure.Contracts.Implementations.Caching;

internal class InMemoryCacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;

    public InMemoryCacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public T? Get<T>(string key)
    {
        return _memoryCache.TryGetValue(key, out T? value) ? value : default!;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        return await Task.FromResult(Get<T>(key));
    }

    public void Remove(string key)
    {
        _memoryCache.Remove(key);
    }

    public Task RemoveAsync(string key, CancellationToken ct = default)
    {
        Remove(key);
        return Task.CompletedTask;
    }

    public void Set<T>(string key, T value, TimeSpan? expiration)
    {
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromHours(1),
            SlidingExpiration = expiration.HasValue
                ? TimeSpan.FromMinutes(Math.Min(expiration.Value.TotalMinutes / 3, 15))
                : TimeSpan.FromMinutes(20),
            Priority = CacheItemPriority.Normal,
            Size = 1
        };

        _memoryCache.Set(key, value, options);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan expiration, CancellationToken ct = default)
    {

        Set(key, value, expiration);

        return Task.CompletedTask;
    }

    public void Set(string cacheKey, DateTimeOffset utcNow, MemoryCacheEntryOptions options)
    {
        _memoryCache.Set(cacheKey, utcNow, options);
    }

    public bool Exists(string key)
    {
        return _memoryCache.TryGetValue(key, out _);
    }

    public void RemoveByPattern(string pattern)
    {
        throw new NotImplementedException();
    }


}

