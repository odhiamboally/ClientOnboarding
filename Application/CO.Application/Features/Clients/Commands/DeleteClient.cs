using CO.Application.Contracts.Interfaces.Common;
using CO.Application.Utilities;
using CO.Domain.Contracts.Interfaces.Common;
using CO.Domain.Entities;
using CO.Shared.Dtos.Client;
using CO.Shared.Dtos.Common;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace CO.Application.Features.Clients.Commands;

/// <summary>
/// Same invalidation pattern as Update: remove the entity entry + bump the list version.
/// </summary>
public record DeleteClientCommand(Guid ClientId) : IRequest<AppResponse<bool>>, ICacheInvalidatorRequest
    
{
    public IReadOnlyList<string> DirectInvalidationKeys => [CacheKeys.Entity("clients", ClientId.ToString())];
    public IReadOnlyList<string> GroupVersionKeysToInvalidate => [CacheKeys.GroupVersion("clients")];
        
}

internal sealed class DeleteClientCommandHandler(IUnitOfWork _unitOfWork, ILogger<DeleteClientCommandHandler> _logger)
    : IRequestHandler<DeleteClientCommand, AppResponse<bool>>
{
    public async Task<AppResponse<bool>> Handle(DeleteClientCommand command, CancellationToken ct)
    {
        try
        {
            var client = await _unitOfWork.ClientRepository.FindByIdAsync(command.ClientId, ct);
            if (client is null)
                return AppResponse<bool>.Failure($"Client {command.ClientId} not found.");

            await _unitOfWork.ClientRepository.SoftDeleteAsync(command.ClientId);

            var saved = await _unitOfWork.CompleteAsync(ct) > 0;
            if (!saved)
                return AppResponse<bool>.Failure("Failed to delete client.");

            return AppResponse<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting client {ClientId}", command.ClientId);
            throw;
        }
        
    }
}
