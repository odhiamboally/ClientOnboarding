using CO.Shared.Dtos.Client;
using CO.Shared.Validation.Validators.Common;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace CO.Shared.Validation.Validators.Clients;

public class UpdateClientRequestValidator : Validator<UpdateClientRequest>
{
    public UpdateClientRequestValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Client ID is required.");

        // Reuse all rules from Create — delegate to avoid duplication
        RuleFor(x => new CreateClientRequest(
            x.ClientType, x.SegmentType, x.SubSegmentType, x.ClientClassification,
            x.CompanyName, x.LineOfBusiness, x.LineOfBusinessMoreInfo, x.NatureOfBusiness,
            x.IdentificationType, x.RegistrationNumber, x.DateOfRegistration,
            x.RegisteredAt, x.RegisteredOffice, x.BusinessStartedYear, x.NumberOfEmployees,
            x.Comments, x.Website, x.TINNumber,
            x.RelationshipManagerId, x.OpenedOn,
            x.ResidentialAddress, x.Country, x.Region, x.Ward, x.District,
            x.BusinessAddress, x.OfficeAddress, x.MailingAddress,
            x.Street, x.ZipCode, x.PhoneHome, x.PhoneWork, x.Mobile,
            x.FaxNo, x.LandMark, x.EmailId,
            x.CanSendGreetings, x.CanSendAssociateSpecialOffer,
            x.CanSendOurSpecialOffers, x.StatementOnline, x.MobileAlert))
            .SetValidator(new CreateClientRequestValidator());
    }
}

