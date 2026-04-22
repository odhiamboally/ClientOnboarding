using CO.Application.Extensions;
using CO.Domain.Entities;
using CO.Shared.Dtos.Client;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace CO.Application.Mappings;

internal static class ClientMappings
{
    public static ClientResponse ToClientResponse(this Client client) =>
        new(
            // Identity & Classification
            client.Id,
            client.ClientNumber,
            client.ClientType.ToDisplayString(),
            client.SegmentType.ToDisplayString(),
            client.SubSegmentType.ToDisplayString(),
            client.Status.ToDisplayString(),
            client.OpenedOn,

            // Corporate Details
            client.CorporateDetail.CompanyName,
            client.CorporateDetail.LineOfBusiness.ToDisplayString(),
            client.CorporateDetail.LineOfBusinessMoreInfo,
            client.CorporateDetail.NatureOfBusiness,
            client.CorporateDetail.IdentificationType.ToDisplayString(),
            client.CorporateDetail.RegistrationNumber,
            client.CorporateDetail.DateOfRegistration,
            client.CorporateDetail.RegisteredAt,
            client.CorporateDetail.RegisteredOffice,
            client.CorporateDetail.BusinessStartedYear,
            client.CorporateDetail.NumberOfEmployees,
            client.CorporateDetail.Comments,
            client.CorporateDetail.Website,
            client.CorporateDetail.TINNumber,

            // Relationship Manager
            client.RelationshipManagerId,
            $"{client.RelationshipManager?.FirstName} {client.RelationshipManager?.LastName}" ?? "—",

            // Address
            client.Address.ResidentialAddress,
            client.Address.Country,
            client.Address.Region,
            client.Address.Ward,
            client.Address.District,
            client.Address.BusinessAddress,
            client.Address.OfficeAddress,
            client.Address.MailingAddress,
            client.Address.Street,
            client.Address.ZipCode,
            client.Address.PhoneHome,
            client.Address.PhoneWork,
            client.Address.Mobile,
            client.Address.FaxNo,
            client.Address.LandMark,
            client.Address.EmailId,

            // Communication Prefs
            client.CommunicationPreference.CanSendGreetings,
            client.CommunicationPreference.CanSendAssociateSpecialOffer,
            client.CommunicationPreference.CanSendOurSpecialOffers,
            client.CommunicationPreference.StatementOnline,
            client.CommunicationPreference.MobileAlert,

            // Directors
            [.. client.Directors.Select(d => d.ToDirectorResponse())]
        );

    public static DirectorResponse ToDirectorResponse(this Director director) =>
        new(
            director.Id,
            director.FullName,
            director.RelationType.ToDisplayString(),
            director.IdentificationType.ToDisplayString(),
            director.IdentificationNumber,
            director.PhoneNumber,
            director.Email,
            director.SharePercentage,
            director.DateAdded
        );

    public static StaffMemberResponse ToStaffMemberResponse(this StaffMember staff) =>
        new(
            staff.Id,
            $"{staff.FirstName}{staff.LastName}",
            staff.StaffNumber,
            staff.Department
        );

    public static Expression<Func<Client, ClientResponse>> AsResponse => client => new ClientResponse(
        client.Id,
        client.ClientNumber,
        client.ClientType.ToDisplayString(), // EF can translate simple ToString() or use a helper
        client.SegmentType.ToDisplayString(),
        client.SubSegmentType.ToDisplayString(),
        client.Status.ToDisplayString(),
        client.OpenedOn,

        // Corporate Details
        client.CorporateDetail.CompanyName,
        client.CorporateDetail.LineOfBusiness.ToDisplayString(),
        client.CorporateDetail.LineOfBusinessMoreInfo,
        client.CorporateDetail.NatureOfBusiness,
        client.CorporateDetail.IdentificationType.ToDisplayString(),
        client.CorporateDetail.RegistrationNumber,
        client.CorporateDetail.DateOfRegistration,
        client.CorporateDetail.RegisteredAt,
        client.CorporateDetail.RegisteredOffice,
        client.CorporateDetail.BusinessStartedYear,
        client.CorporateDetail.NumberOfEmployees,
        client.CorporateDetail.Comments,
        client.CorporateDetail.Website,
        client.CorporateDetail.TINNumber,
        client.RelationshipManagerId,
        client.RelationshipManager != null ? $"{client.RelationshipManager.FirstName} {client.RelationshipManager.LastName}" : "—",

        // Address
        client.Address.ResidentialAddress,
        client.Address.Country,
        client.Address.Region,
        client.Address.Ward,
        client.Address.District,
        client.Address.BusinessAddress,
        client.Address.OfficeAddress,
        client.Address.MailingAddress,
        client.Address.Street,
        client.Address.ZipCode,
        client.Address.PhoneHome,
        client.Address.PhoneWork,
        client.Address.Mobile,
        client.Address.FaxNo,
        client.Address.LandMark,
        client.Address.EmailId,

        // Communication Prefs
        client.CommunicationPreference.CanSendGreetings,
        client.CommunicationPreference.CanSendAssociateSpecialOffer,
        client.CommunicationPreference.CanSendOurSpecialOffers,
        client.CommunicationPreference.StatementOnline,
        client.CommunicationPreference.MobileAlert,

        client.Directors.Select(d => new DirectorResponse(
            d.Id,
            d.FullName,
            d.RelationType.ToDisplayString(),
            d.IdentificationType.ToDisplayString(),
            d.IdentificationNumber,
            d.PhoneNumber,
            d.Email,
            d.SharePercentage,
            d.DateAdded)).ToList()
    );

    
}
