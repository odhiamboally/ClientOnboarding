using CO.Shared.Dtos.Client;

namespace CO.UI.Blazor.Features.Clients.Models;

public class ClientFormModel
{
    // ── Classification ───────────────────────────────────────────────────
    public string ClientType { get; set; } = string.Empty;
    public string SegmentType { get; set; } = string.Empty;
    public string SubSegmentType { get; set; } = string.Empty;
    public string? ClientClassification { get; set; }

    // ── Corporate Details ────────────────────────────────────────────────
    public string CompanyName { get; set; } = string.Empty;
    public string LineOfBusiness { get; set; } = string.Empty;
    public string? LineOfBusinessMoreInfo { get; set; }
    public string NatureOfBusiness { get; set; } = string.Empty;
    public string IdentificationType { get; set; } = string.Empty;
    public string RegistrationNumber { get; set; } = string.Empty;
    public DateTimeOffset DateOfRegistration { get; set; } = DateTimeOffset.UtcNow;
    public string? RegisteredAt { get; set; }
    public string? RegisteredOffice { get; set; }
    public int? BusinessStartedYear { get; set; }
    public int? NumberOfEmployees { get; set; }
    public string? Comments { get; set; }
    public string? Website { get; set; }
    public string? TINNumber { get; set; }

    // ── RM & Opening ─────────────────────────────────────────────────────
    public Guid RelationshipManagerId { get; set; }
    public DateTimeOffset OpenedOn { get; set; } = DateTimeOffset.UtcNow;

    // ── Address ──────────────────────────────────────────────────────────
    public string ResidentialAddress { get; set; } = string.Empty;
    public string Country { get; set; } = "Tanzania";
    public string Region { get; set; } = string.Empty;
    public string Ward { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string? BusinessAddress { get; set; }
    public string? OfficeAddress { get; set; }
    public string? MailingAddress { get; set; }
    public string? Street { get; set; }
    public string? ZipCode { get; set; }
    public string? PhoneHome { get; set; }
    public string? PhoneWork { get; set; }
    public string? Mobile { get; set; }
    public string? FaxNo { get; set; }
    public string? LandMark { get; set; }
    public string? EmailId { get; set; }

    // ── Communication Prefs ──────────────────────────────────────────────
    public bool CanSendGreetings { get; set; }
    public bool CanSendAssociateSpecialOffer { get; set; }
    public bool CanSendOurSpecialOffers { get; set; }
    public bool StatementOnline { get; set; }
    public bool MobileAlert { get; set; }

    // ── Mapping ──────────────────────────────────────────────────────────
    public CreateClientRequest ToCreateRequest() => new(
        ClientType, SegmentType, SubSegmentType, ClientClassification,
        CompanyName, LineOfBusiness, LineOfBusinessMoreInfo, NatureOfBusiness,
        IdentificationType, RegistrationNumber, DateOfRegistration,
        RegisteredAt, RegisteredOffice, BusinessStartedYear, NumberOfEmployees,
        Comments, Website, TINNumber, RelationshipManagerId, OpenedOn,
        ResidentialAddress, Country, Region, Ward, District,
        BusinessAddress, OfficeAddress, MailingAddress,
        Street, ZipCode, PhoneHome, PhoneWork, Mobile, FaxNo, LandMark, EmailId,
        CanSendGreetings, CanSendAssociateSpecialOffer, CanSendOurSpecialOffers,
        StatementOnline, MobileAlert
    );

    public UpdateClientRequest ToUpdateRequest(Guid id) => new(
        id, ClientType, SegmentType, SubSegmentType, ClientClassification,
        CompanyName, LineOfBusiness, LineOfBusinessMoreInfo, NatureOfBusiness,
        IdentificationType, RegistrationNumber, DateOfRegistration,
        RegisteredAt, RegisteredOffice, BusinessStartedYear, NumberOfEmployees,
        Comments, Website, TINNumber, RelationshipManagerId, OpenedOn,
        ResidentialAddress, Country, Region, Ward, District,
        BusinessAddress, OfficeAddress, MailingAddress,
        Street, ZipCode, PhoneHome, PhoneWork, Mobile, FaxNo, LandMark, EmailId,
        CanSendGreetings, CanSendAssociateSpecialOffer, CanSendOurSpecialOffers,
        StatementOnline, MobileAlert
    );

    public static ClientFormModel FromResponse(ClientResponse r) => new()
    {
        ClientType = r.ClientType,
        SegmentType = r.SegmentType,
        SubSegmentType = r.SubSegmentType,
        ClientClassification = null, // not in ClientResponse — add if needed
        CompanyName = r.CompanyName,
        LineOfBusiness = r.LineOfBusiness,
        LineOfBusinessMoreInfo = r.LineOfBusinessMoreInfo,
        NatureOfBusiness = r.NatureOfBusiness ?? string.Empty,
        IdentificationType = r.IdentificationType,
        RegistrationNumber = r.RegistrationNumber,
        DateOfRegistration = r.DateOfRegistration,
        RegisteredAt = r.RegisteredAt,
        RegisteredOffice = r.RegisteredOffice,
        BusinessStartedYear = r.BusinessStartedYear,
        NumberOfEmployees = r.NumberOfEmployees,
        Comments = r.Comments,
        Website = r.Website,
        TINNumber = r.TINNumber,
        RelationshipManagerId = r.RelationshipManagerId,
        OpenedOn = r.OpenedOn,
        ResidentialAddress = r.ResidentialAddress ?? string.Empty,
        Country = r.Country ?? "Tanzania",
        Region = r.Region ?? string.Empty,
        Ward = r.Ward ?? string.Empty,
        District = r.District ?? string.Empty,
        BusinessAddress = r.BusinessAddress,
        OfficeAddress = r.OfficeAddress,
        MailingAddress = r.MailingAddress,
        Street = r.Street,
        ZipCode = r.ZipCode,
        PhoneHome = r.PhoneHome,
        PhoneWork = r.PhoneWork,
        Mobile = r.Mobile,
        FaxNo = r.FaxNo,
        LandMark = r.LandMark,
        EmailId = r.EmailId,
        CanSendGreetings = r.CanSendGreetings,
        CanSendAssociateSpecialOffer = r.CanSendAssociateSpecialOffer,
        CanSendOurSpecialOffers = r.CanSendOurSpecialOffers,
        StatementOnline = r.StatementOnline,
        MobileAlert = r.MobileAlert
    };
}
