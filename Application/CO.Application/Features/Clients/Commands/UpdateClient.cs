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
using System;
using System.Collections.Generic;
using System.Text;

namespace CO.Application.Features.Clients.Commands;


/// <summary>
/// Invalidation:
///   - Direct:  delete the entity entry so the next GetById call fetches fresh data.
///   - Version: bump the global "clients" version to orphan all list entries.
///
/// Both are necessary: without the direct deletion the entity detail page would
/// still show stale data even after the list refreshes.
/// </summary>
public record UpdateClientCommand(Guid Id, UpdateClientRequest UpdateClientRequest, string UserId) 
    : IRequest<AppResponse<ClientResponse>>, ICacheInvalidatorRequest
{
    public IReadOnlyList<string> DirectInvalidationKeys => [CacheKeys.Entity("clients", Id.ToString())];
    public IReadOnlyList<string> GroupVersionKeysToInvalidate => [CacheKeys.GroupVersion("clients")];
        
}

internal sealed class UpdateClientCommandHandler(IUnitOfWork _unitOfWork, ILogger<UpdateClientCommandHandler> _logger) 
    : IRequestHandler<UpdateClientCommand, AppResponse<ClientResponse>>
{
    
    public async Task<AppResponse<ClientResponse>> Handle(UpdateClientCommand command, CancellationToken ct)
    {
        try
        {
            var req = command.UpdateClientRequest;

            var client = await _unitOfWork.ClientRepository.FindByIdAsync(req.Id, ct);
            if (client is null)
                return AppResponse<ClientResponse>.Failure($"Client {req.Id} not found.");
                    
            // Verify new RM if it changed
            var rm = await _unitOfWork.StaffMemberRepository.FindByIdAsync(req.RelationshipManagerId, ct);
            if (rm is null || rm.IsDeleted)
                return AppResponse<ClientResponse>.Failure("Selected Relationship Manager does not exist or is inactive.");
                    

            // Update via aggregate behaviours — never set properties directly
            client.SetCorporateDetails(CorporateDetail.Create(
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
                
            ));

            client.SetAddress(Address.Create(
                residentialAddress: req.ResidentialAddress,
                country: req.Country,
                region: req.Region,
                ward: req.Ward,
                district: req.District,
                mobile: req.Mobile,
                emailId: req.EmailId,
                businessAddress: req.BusinessAddress,
                officeAddress: req.OfficeAddress,
                mailingAddress: req.MailingAddress,
                street: req.Street,
                zipCode: req.ZipCode,
                phoneHome: req.PhoneHome,
                phoneWork: req.PhoneWork,
                faxNo: req.FaxNo,
                landMark: req.LandMark
            ));

            client.SetCommunicationPreferences(CommunicationPreference.Create(
                canSendGreetings: req.CanSendGreetings,
                canSendAssociateSpecialOffer: req.CanSendAssociateSpecialOffer,
                canSendOurSpecialOffers: req.CanSendOurSpecialOffers,
                statementOnline: req.StatementOnline,
                mobileAlert: req.MobileAlert
            ));

            client.AssignRelationshipManager(req.RelationshipManagerId);

            await _unitOfWork.ClientRepository.UpdateAsync(client);
            await _unitOfWork.CompleteAsync(ct);

            _logger.LogInformation("Client updated: {ClientNumber}", client.ClientNumber);

            return AppResponse<ClientResponse>.Success("Client updated successfully.", client.ToClientResponse());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating client {ClientId}", command.UpdateClientRequest.Id);
            throw;
        }
    }
}
