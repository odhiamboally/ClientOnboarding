using CO.Shared.Dtos.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace CO.Application.Utilities;

public static class CacheKeys
{
    

    public static string CurrentUser() => $"CurrentUser";


    // The version sentinel - this is what you invalidate on mutations
    public static string ClientListVersion(string userId) => $"clients:version:{userId}";
    public static string ClientList(string userId, string version, ClientSearchRequest req)
        => $"clients:list:{userId}:{version}:{ComputeFilterHash(req)}";

    public static string ClientById(Guid Id) => $"clients:entity:{Id}";

    public static string StaffMemberList(string userId) => $"staff-members:list:{userId}";

    internal static string ComputeFilterHash(ClientSearchRequest req)
    {
        var raw = $"{req.GlobalSearch}|{req.ClientType}|{req.SegmentType}" +
                  $"|{req.RelationshipManagerId}|{req.Cursor}|{req.PageSize}";

        var bytes = System.Security.Cryptography.SHA256.HashData(Encoding.UTF8.GetBytes(raw));
            
        return Convert.ToHexString(bytes)[..16].ToLowerInvariant();
    }

   
}

