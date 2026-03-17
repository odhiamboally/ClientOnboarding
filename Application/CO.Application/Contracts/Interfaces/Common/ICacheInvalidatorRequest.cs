using System;
using System.Collections.Generic;
using System.Text;

namespace CO.Application.Contracts.Interfaces.Common;

public interface ICacheInvalidatorRequest
{
    // Keys to delete directly (entity-level)
    List<string> CacheKeysToInvalidate => [];

    // Version keys to bump (list-level — invalidates all filter variants)  
    List<string> CacheVersionKeysToInvalidate => [];
}
