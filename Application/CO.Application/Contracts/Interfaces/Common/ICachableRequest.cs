using System;
using System.Collections.Generic;
using System.Text;

namespace CO.Application.Contracts.Interfaces.Common;

public interface ICachableRequest
{
    string CacheKeyPrefix { get; } // identifies the "family" of keys
    string CacheKeySuffix { get; } // the filter-specific part
    TimeSpan? Expiration => TimeSpan.FromMinutes(30);
    bool ShouldCache => true;
    bool IsVersioned => false;
    
}
