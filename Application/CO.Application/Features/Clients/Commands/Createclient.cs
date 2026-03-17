using AutoMapper;
using CO.Application.Contracts.Interfaces.Common;
using CO.Application.Extensions;
using CO.Application.Mappings;
using CO.Application.Utilities;
using CO.Domain.Contracts.Interfaces.Common;
using CO.Domain.Entities;
using CO.Domain.Enums;
using CO.Domain.ValueObjects;
using CO.Shared.Dtos.Client;
using CO.Shared.Dtos.Common;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace CO.Application.Features.Clients.Commands;

/// <summary>
/// Invalidation: bump the version token for "clients" globally.
/// No entity key to delete (the entity does not exist in cache yet).
/// Every user's versioned list entries are orphaned in O(1).
/// </summary>
public record CreateClientCommand(CreateClientRequest CreateClientRequest, string UserId) 
    : IRequest<AppResponse<ClientResponse>>, ICacheInvalidatorRequest
{
    // No direct keys — new entity, nothing cached yet.
    public IReadOnlyList<string> DirectInvalidationKeys => [];

    // Bump the global "clients" version so all users see the new entry on next list load.
    public IReadOnlyList<string> GroupVersionKeysToInvalidate => [CacheKeys.GroupVersion("clients")];
        

}
    
internal sealed class CreateClientCommandHandler(
    IUnitOfWork _unitOfWork,
    IClientNumberGenerator _clientNumberGenerator, 
    ILogger<CreateClientCommandHandler> _logger) : IRequestHandler<CreateClientCommand, AppResponse<ClientResponse>>
{
    public async Task<AppResponse<ClientResponse>> Handle(CreateClientCommand command, CancellationToken ct)
    {
        try
        {
            var req = command.CreateClientRequest;

            var clientNumber = await _clientNumberGenerator.GenerateAsync(ct);

            // Build owned entities
            var corporateDetails = CorporateDetail.Create(
                req.CompanyName,
                req.LineOfBusiness.ToEnum<LineOfBusiness>(),
                req.NatureOfBusiness,
                req.IdentificationType.ToEnum<IdentificationType>(),
                req.RegistrationNumber,
                req.DateOfRegistration,
                req.LineOfBusinessMoreInfo,
                req.RegisteredAt,
                req.RegisteredOffice,
                req.BusinessStartedYear,
                req.NumberOfEmployees,
                req.Website,
                req.TINNumber,
                req.ClientClassification,
                req.Comments
                
            );

            var address = Address.Create(
                req.ResidentialAddress,
                req.Country,
                req.Region,
                req.Ward,
                req.District,
                req.Mobile,
                req.EmailId,
                req.BusinessAddress,
                req.OfficeAddress,
                req.MailingAddress,
                req.Street,
                req.ZipCode,
                req.PhoneHome,
                req.PhoneWork,
                req.FaxNo,
                req.LandMark
            );

            var communicationPrefs = CommunicationPreference.Create(
                canSendGreetings: req.CanSendGreetings,
                canSendAssociateSpecialOffer: req.CanSendAssociateSpecialOffer,
                canSendOurSpecialOffers: req.CanSendOurSpecialOffers,
                statementOnline: req.StatementOnline,
                mobileAlert: req.MobileAlert
            );

            // Create aggregate root — domain rules enforced inside
            var client = Client.Create(
                clientNumber,
                req.CompanyName,
                req.ClientType.ToEnum<ClientType>(),
                req.SegmentType.ToEnum<SegmentType>(),
                req.SubSegmentType.ToEnum<SubSegmentType>(),
                req.RelationshipManagerId,
                req.OpenedOn,
                corporateDetails,
                address,
                communicationPrefs
            );

            await _unitOfWork.ClientRepository.CreateAsync(client, ct);
            await _unitOfWork.CompleteAsync(ct); // triggers domain event dispatch

            _logger.LogInformation("Client created: {ClientNumber} — {CompanyName}", client.ClientNumber, client.CorporateDetail.CompanyName);

            return AppResponse<ClientResponse>.Success(
                $"Client {client.ClientNumber} created successfully.",
                client.ToClientResponse());
                
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Domain validation failed creating client");
            return AppResponse<ClientResponse>.Failure(ex.Message);  
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating client");
            throw;  
        }
    }
}
