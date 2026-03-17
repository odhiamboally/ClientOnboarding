using CO.Application.Contracts.Interfaces.Common;
using CO.Application.Mappings;
using CO.Application.Utilities;
using CO.Domain.Contracts.Interfaces.Common;
using CO.Shared.Dtos.Client;
using CO.Shared.Dtos.Common;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace CO.Application.Features.Clients.Queries;

/// <summary>
/// Fetches a single client by ID.
///
/// Cache strategy — NON-VERSIONED entity entry:
///   Key:  "clients:entity:{id}"
///   TTL:  30 minutes
///   Scope: global (entity data is not user-specific at the query level)
///
/// Invalidation:
///   UpdateClientCommand and DeleteClientCommand must include
///   CacheKeys.Entity("clients", id) in their DirectInvalidationKeys.
/// </summary>
public record GetClientByIdQuery(Guid Id) : IRequest<AppResponse<ClientResponse>>, ICachableRequest
{
    public string CacheGroup => "clients";
    public string CacheDiscriminator => Id.ToString();
    public string? CacheUserId => null;   // entity cache is shared across users
    public bool IsVersioned => false;  // invalidated directly by exact key
}

internal sealed class GetClientByIdQueryHandler(
    IUnitOfWork _unitOfWork,
    ILogger<GetClientByIdQueryHandler> _logger)
    : IRequestHandler<GetClientByIdQuery, AppResponse<ClientResponse>>
{
    public async Task<AppResponse<ClientResponse>> Handle(GetClientByIdQuery query, CancellationToken ct)
    {
        try
        {
            var client = await _unitOfWork.ClientRepository.FindByIdAsync(query.Id, ct);

            if (client is null)
                return AppResponse<ClientResponse>.Failure($"Client with ID {query.Id} was not found.");

            return AppResponse<ClientResponse>.Success(client.ToClientResponse());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching client {ClientId}", query.Id);
            throw;
        }
    }
}
