using CO.Shared.Dtos.Client;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace CO.Application.Utilities;

/// <summary>
/// Single source of truth for every cache key pattern in the application.
///
/// Key anatomy:
///   Entity (non-versioned):  "{group}:entity:{discriminator}"
///   List   (versioned):      "{group}:list:{scope}:{versionToken}:{discriminator}"
///   Version sentinel:        "{group}:version:{scope}"
///
/// Where {scope} is either the userId or the literal "global".
///
/// Rules:
///   - Only this class builds keys. Behaviors and queries call helpers here.
///   - Hashing is stable (SHA-256, hex-encoded, trimmed to 16 chars).
///   - All keys are lowercase to prevent accidental duplicates.
/// </summary>
public static class CacheKeys
{
    // ── Public key builders ────────────────────────────────────────────────────

    /// <summary>
    /// Version sentinel key for a cache group.
    /// Bumping this key orphans every versioned list entry in the group.
    /// </summary>
    /// <param name="group">e.g. "clients"</param>
    /// <param name="userId">Null for a global (cross-user) version token.</param>
    public static string GroupVersion(string group, string? userId = null)
        => userId is null
            ? $"{group}:version:global"
            : $"{group}:version:{userId.ToLowerInvariant()}";

    /// <summary>
    /// Non-versioned key for a single entity lookup.
    /// Invalidated directly by its exact key when the entity is mutated.
    /// </summary>
    /// <param name="group">e.g. "clients"</param>
    /// <param name="id">String form of the entity identifier.</param>
    public static string Entity(string group, string id) => $"{group}:entity:{id.ToLowerInvariant()}";


    /// <summary>
    /// Assembles the full versioned list key.
    /// Called by <see cref="Behaviours.CachingBehavior{TRequest,TResponse}"/>
    /// after it has resolved the version token — not by queries directly.
    /// </summary>
    internal static string VersionedList(string group, string scope, string versionToken, string discriminator)
        => $"{group}:list:{scope}:{versionToken}:{discriminator}";

    // ── Discriminator helpers ──────────────────────────────────────────────────

    /// <summary>
    /// Produces a stable, compact discriminator for any filter object.
    /// Serialises the object properties in a deterministic order using
    /// <paramref name="raw"/> (the caller composes the canonical string).
    ///
    /// Returns the first 16 hex characters of the SHA-256 hash — enough
    /// uniqueness for any realistic filter space (2^64 values).
    /// </summary>
    public static string HashFilter(string raw)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(raw));
        return Convert.ToHexString(bytes)[..16].ToLowerInvariant();
    }

    // ── Named discriminator builders (one per filterable entity) ──────────────

    /// <summary>Discriminator for a client list/search query.</summary>
    public static string ClientListDiscriminator(
        string? globalSearch,
        string? clientType,
        string? segmentType,
        string? status,
        Guid? relationshipManagerId,
        Guid? cursor,
        int pageSize)
        => HashFilter($"{globalSearch}|{clientType}|{segmentType}|{status}|{relationshipManagerId}|{cursor}|{pageSize}");
        
    /// <summary>Discriminator for a staff member list query.</summary>
    public static string StaffListDiscriminator(Guid? cursor, int pageSize)
        => HashFilter($"{cursor}|{pageSize}");
}
