using CO.Application.Contracts.Interfaces.Common;
using CO.Application.Extensions;
using CO.Application.Mappings;
using CO.Application.Utilities;
using CO.Domain.Contracts.Implementations.Specifications;
using CO.Domain.Contracts.Interfaces.Common;
using CO.Domain.Enums;
using CO.Shared.Dtos.Client;
using CO.Shared.Dtos.Common;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace CO.Application.Features.Clients.Queries;

public record SearchClientListQuery(ClientSearchRequest SearchRequest, string UserId)
    : IRequest<AppResponse<PagedResponse<ClientResponse, Guid>>>, ICachableRequest
{

    public string CacheGroup => "clients";
    public string CacheDiscriminator => 
        CacheKeys.ClientListDiscriminator(
            SearchRequest.GlobalSearch,
            SearchRequest.ClientType,
            SearchRequest.SegmentType,
            SearchRequest.RelationshipManagerId,
            SearchRequest.Cursor,
            SearchRequest.PageSize);

    public string? CacheUserId => UserId;
    public bool IsVersioned => true;
    public bool BypassCache => false;  // explicit; see XML doc above
}

internal sealed class SearchClientListQueryHandler(IUnitOfWork _unitOfWork, ILogger<SearchClientListQueryHandler> _logger)
    : IRequestHandler<SearchClientListQuery, AppResponse<PagedResponse<ClientResponse, Guid>>>
{
    public async Task<AppResponse<PagedResponse<ClientResponse, Guid>>> Handle(SearchClientListQuery query, CancellationToken ct)
    {
        try
        {
            var req = query.SearchRequest;

            var pageSize = Math.Clamp(req.PageSize, 1, 50);

            var clientType = req.ClientType?.ToEnum<ClientType>();
            var segmentType = req.SegmentType?.ToEnum<SegmentType>();
            var subSegmentType = req.SubSegmentType?.ToEnum<SubSegmentType>();
            var identificationType = req.IdentificationType?.ToEnum<IdentificationType>();
            var lineOfBusiness = req.LineOfBusiness?.ToEnum<LineOfBusiness>();
            var status = req.Status?.ToEnum<ClientStatus>();

            var spec = new ClientSearchSpec(
                req.GlobalSearch,
                clientType,
                segmentType,
                subSegmentType,
                identificationType,
                lineOfBusiness,
                status,
                req.RelationshipManagerId,
                req.Cursor,
                pageSize

            );

            var totalCount = await _unitOfWork.ClientRepository.CountAsync(spec, ct);
            var clientEntities = await _unitOfWork.ClientRepository.SearchAsync(spec, ct);

            bool hasNextPage = clientEntities.Count > pageSize;
            if (hasNextPage)
                clientEntities.RemoveAt(clientEntities.Count - 1);

            var items = clientEntities.Select(x => x.ToClientResponse()).ToList();

            var nextCursor = hasNextPage ? items[^1].Id : (Guid?)null;

            // Display sort — purely cosmetic, does not affect pagination
            items = [.. items.OrderBy(x => x.ClientNumber, StringComparer.OrdinalIgnoreCase)];

            bool isFirstPage = req.Cursor == null || req.Cursor == Guid.Empty;

            var pagedResult = new PagedResponse<ClientResponse, Guid>(
                items,
                totalCount,
                1,
                pageSize,
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
