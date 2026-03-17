using CO.Application.Contracts.Interfaces.Common;
using CO.Application.Extensions;
using CO.Application.Mappings;
using CO.Application.Utilities;
using CO.Domain.Contracts.Implementations.Specifications;
using CO.Domain.Contracts.Interfaces.Common;
using CO.Domain.Contracts.Specifications;
using CO.Domain.Entities;
using CO.Domain.Enums;
using CO.Shared.Dtos.Client;
using CO.Shared.Dtos.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace CO.Application.Features.Clients.Queries;


// ════════════════════════════════════════════════════════════════════════════════
//  GET CLIENT LIST  (unfiltered, paginated)
// ════════════════════════════════════════════════════════════════════════════════
 
/// <summary>
/// Returns the full client list with cursor-based pagination, no search filters.
///
/// Cache strategy — VERSIONED list entry:
///   Key:  "clients:list:{userId}:{versionToken}:{discriminator}"
///   TTL:  30 minutes
///   Scope: per user — each RM sees only their relevant data
///
/// Invalidation: any mutation command bumps CacheKeys.GroupVersion("clients", userId),
/// which orphans every versioned entry for that user in O(1).
/// </summary>
public record GetClientListQuery(ClientListRequest ClientListRequest, string UserId)
    : IRequest<AppResponse<PagedResponse<ClientResponse, Guid>>>, ICachableRequest
{
    public string CacheGroup => "clients";
    public string CacheDiscriminator => 
        CacheKeys.ClientListDiscriminator(
            globalSearch: null,
            clientType: null,
            segmentType: null,
            relationshipManagerId: null,
            cursor: ClientListRequest.Cursor,
            pageSize: ClientListRequest.PageSize);

    public string? CacheUserId => UserId;
    public bool IsVersioned => true;
}

internal sealed class GetClientListQueryHandler(IUnitOfWork _unitOfWork, ILogger<GetClientListQueryHandler> _logger) 
    : IRequestHandler<GetClientListQuery, AppResponse<PagedResponse<ClientResponse, Guid>>>
{
    public async Task<AppResponse<PagedResponse<ClientResponse, Guid>>> Handle(GetClientListQuery query, CancellationToken ct)
    {
        try
        {
            var req = query.ClientListRequest;

            // Enforce page size bounds regardless of what the caller sent.
            // Min 1 — reject nonsense values.
            // Max 50 — protect the DB and cache from oversized result sets.
            var pageSize = Math.Clamp(req.PageSize, 1, 50);

            var cursor = req.Cursor;

            var totalCount = await _unitOfWork.ClientRepository.CountAsync(ct);
            var clientEntities = await _unitOfWork.ClientRepository
                .FindAll()
                .Where(c => req.Cursor == null || req.Cursor == Guid.Empty || c.Id > req.Cursor)
                .OrderBy(c => c.Id)
                .Take(req.PageSize + 1)
                .ToListAsync(ct);

            var hasNextPage = clientEntities.Count > req.PageSize;

            if (hasNextPage)
                clientEntities.RemoveAt(clientEntities.Count - 1);

            var items = clientEntities.Select(c => c.ToClientResponse()).ToList();
            var nextCursor = hasNextPage ? items.LastOrDefault()?.Id : null;
            var nextCursor1 = hasNextPage ? items[^1].Id : (Guid?)null;

            bool isFirstPage = req.Cursor == null || req.Cursor == Guid.Empty;

            var pagedResult = new PagedResponse<ClientResponse, Guid>(
                items,
                totalCount,
                1,
                req.PageSize,
                isFirstPage,
                nextCursor ?? Guid.Empty
            );

            return AppResponse<PagedResponse<ClientResponse, Guid>>.Success(pagedResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching client list");
            throw;
        }
    }
}

