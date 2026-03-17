using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace CO.Application.Contracts.Interfaces.Common;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken ct = default);
    T? Get<T>(string key);

    Task SetAsync<T>(string key, T value, TimeSpan expiration, CancellationToken ct = default);
    void Set(string cacheKey, DateTimeOffset utcNow, MemoryCacheEntryOptions options);
    void Set<T>(string key, T value, TimeSpan? expiration);


    Task RemoveAsync(string key, CancellationToken ct = default);
    void Remove(string key);
    void RemoveByPattern(string pattern);


    bool Exists(string key);


}
