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

public record GetClientListQuery(ClientSearchRequest SearchRequest, string UserId) 
    : IRequest<AppResponse<PagedResponse<ClientResponse, Guid>>>, ICachableRequest
{
    
    public string CacheKeyPrefix => CacheKeys.ClientListVersion(UserId);
    public string CacheKeySuffix => CacheKeys.ComputeFilterHash(SearchRequest);
    public bool IsVersioned => true;
    public bool ShouldCache => !IsFiltered;


    private bool IsFiltered =>
        !string.IsNullOrWhiteSpace(SearchRequest.GlobalSearch) ||
        SearchRequest.ClientType is not null ||
        SearchRequest.RelationshipManagerId is not null;
}

internal sealed class GetClientListQueryHandler(IUnitOfWork _unitOfWork, ILogger<GetClientListQueryHandler> _logger) 
    : IRequestHandler<GetClientListQuery, AppResponse<PagedResponse<ClientResponse, Guid>>>
{
    public async Task<AppResponse<PagedResponse<ClientResponse, Guid>>> Handle(GetClientListQuery query, CancellationToken ct)
    {
        try
        {
            var req = query.SearchRequest;

            var clientType = req.ClientType?.ToEnum<ClientType>();
            var segmentType = req.ClientType?.ToEnum<SegmentType>();
            var subSegmentType = req.ClientType?.ToEnum<SubSegmentType>();
            var identificationType = req.ClientType?.ToEnum<IdentificationType>();
            var lineOfBusiness = req.ClientType?.ToEnum<LineOfBusiness>();
            var status = req.ClientType?.ToEnum<ClientStatus>();
            Guid? cursor = req.Cursor;

            var spec = new ClientSearchSpec(
                req.GlobalSearch,
                clientType,
                segmentType,
                subSegmentType,
                identificationType,
                lineOfBusiness,
                status,
                req.RelationshipManagerId,
                cursor,
                req.PageSize

            );

            // Note: You might want a CountAsync method that accepts a spec if you need filtered counts
            var totalCount = await _unitOfWork.ClientRepository.CountAsync(ct);
            var clientEntities = await _unitOfWork.ClientRepository.SearchAsync(spec, ct);

            // Check if we got the "N + 1" record
            bool hasNextPage = clientEntities.Count > req.PageSize;

            // If we have an extra record, remove it from the list so we only return the requested PageSize
            if (hasNextPage)
            {
                clientEntities.RemoveAt(clientEntities.Count - 1);
            }

            var items = clientEntities.Select(x => x.ToClientResponse()).ToList();

            // Determine Next Cursor: If no next page, return null to tell UI to disable "Next"
            var nextCursor = hasNextPage ? items.LastOrDefault()?.Id : null;

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

