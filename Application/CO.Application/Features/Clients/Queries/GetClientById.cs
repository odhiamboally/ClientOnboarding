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

public record GetClientByIdQuery(Guid Id) : IRequest<AppResponse<ClientResponse>>, ICachableRequest
{
    public string CacheKeyPrefix => CacheKeys.ClientById(Id);
    public string CacheKeySuffix => string.Empty;
    public bool IsVersioned => false;  
}

internal sealed class GetClientByIdQueryHandler(IUnitOfWork _unitOfWork,ILogger<GetClientByIdQueryHandler> _logger) 
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
