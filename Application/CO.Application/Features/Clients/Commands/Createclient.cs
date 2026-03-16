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

public record CreateClientCommand(CreateClientRequest CreateClientRequest, string UserId) 
    : IRequest<AppResponse<ClientResponse>>, ICacheInvalidatorRequest
{
    public List<string> CacheVersionKeysToInvalidate => [CacheKeys.ClientListVersion(UserId)];
        
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating client");
            throw;
        }
    }
}
