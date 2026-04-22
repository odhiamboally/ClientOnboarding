using CO.Application.Contracts.Interfaces.Common;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace CO.Infrastructure.Contracts.Implementations.Caching;


/// <summary>
/// IMemoryCache-backed implementation of ICacheService.
/// Suitable for single-node deployments. For multi-instance/distributed
/// deployments, replace with a Redis-backed implementation.
///
/// Key tracking:
///   IMemoryCache has no key enumeration API, so RemoveByPrefixAsync maintains
///   a ConcurrentDictionary of active keys. This adds a small memory overhead
///   but is necessary for prefix-based removal.
///   In practice you should rarely need RemoveByPrefixAsync — the version-token
///   invalidation strategy makes prefix scanning unnecessary for most scenarios.
/// </summary>
internal sealed class InMemoryCacheService : ICacheService
{
    private readonly IMemoryCache _cache;

    // Tracks all keys currently set through this service.
    // Used exclusively by RemoveByPrefixAsync.
    private readonly ConcurrentDictionary<string, byte> _keys = new();

    public InMemoryCacheService(IMemoryCache cache)
    {
        _cache = cache;
    }

    // ── Read ──────────────────────────────────────────────────────────────────

    public Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        var value = _cache.TryGetValue(key, out T? cached) ? cached : default;
        return Task.FromResult(value);
    }

    public Task<bool> ExistsAsync(string key, CancellationToken ct = default)
        => Task.FromResult(_cache.TryGetValue(key, out _));

    // ── Write ─────────────────────────────────────────────────────────────────

    public Task SetAsync<T>(string key, T value, TimeSpan expiration, CancellationToken ct = default)
    {
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration,

            // Sliding window = ⅓ of TTL, capped at 15 min.
            // Keeps frequently-accessed entries warm without extending them indefinitely.
            SlidingExpiration = TimeSpan.FromMinutes(
                Math.Min(expiration.TotalMinutes / 3, 15)),

            Priority = CacheItemPriority.Normal,
            Size = 1
        }
        .RegisterPostEvictionCallback((evictedKey, _, _, _) =>
        {
            // Clean up the key tracker when the entry expires or is evicted.
            _keys.TryRemove(evictedKey.ToString()!, out _);
        });

        _cache.Set(key, value, options);
        _keys[key] = 0;

        return Task.CompletedTask;
    }

    // ── Invalidate ────────────────────────────────────────────────────────────

    public Task RemoveAsync(string key, CancellationToken ct = default)
    {
        _cache.Remove(key);
        _keys.TryRemove(key, out _);
        return Task.CompletedTask;
    }

    public Task RemoveByPrefixAsync(string prefix, CancellationToken ct = default)
    {
        // Snapshot the keys first to avoid modifying the collection mid-iteration.
        var matches = _keys.Keys
            .Where(k => k.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            .ToList();

        foreach (var key in matches)
        {
            _cache.Remove(key);
            _keys.TryRemove(key, out _);
        }

        return Task.CompletedTask;
    }
}


