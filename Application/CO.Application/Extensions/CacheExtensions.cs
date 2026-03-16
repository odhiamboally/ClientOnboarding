using CO.Application.Contracts.Interfaces.Common;
using CO.Application.Utilities;
using CO.Shared.Dtos.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace CO.Application.Extensions;

public static class CacheExtensions
{
    extension(ICacheService _cache)
    {
        public List<ClientResponse>? GetClientList(string appUserId, string version, ClientSearchRequest searchRequest)
        {
            return _cache.Get<List<ClientResponse>>(CacheKeys.ClientList(appUserId, version, searchRequest));
        }
        
    }
}
