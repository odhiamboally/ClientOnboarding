using System;
using System.Collections.Generic;
using System.Text;

namespace CO.Shared.Dtos.Client;

public record CreateClientRequest(

    // ── Classification ──────────────────────────────────────────────────
    string ClientType,
    string SegmentType,
    string SubSegmentType,
    string? ClientClassification,

    // ── Corporate Details ────────────────────────────────────────────────
    string CompanyName,
    string LineOfBusiness,
    string? LineOfBusinessMoreInfo,
    string NatureOfBusiness,
    string IdentificationType,
    string RegistrationNumber,
    DateTimeOffset DateOfRegistration,
    string? RegisteredAt,
    string? RegisteredOffice,
    int? BusinessStartedYear,
    int? NumberOfEmployees,
    string? Comments,
    string? Website,
    string? TINNumber,

    // ── Relationship Manager & Opening ──────────────────────────────────
    Guid RelationshipManagerId,
    DateTimeOffset OpenedOn,

    // ── Address ─────────────────────────────────────────────────────────
    string ResidentialAddress,
    string Country,
    string Region,
    string Ward,
    string District,
    string? BusinessAddress,
    string? OfficeAddress,
    string? MailingAddress,
    string? Street,
    string? ZipCode,
    string? PhoneHome,
    string? PhoneWork,
    string? Mobile,
    string? FaxNo,
    string? LandMark,
    string? EmailId,

    // ── Communication Preferences ────────────────────────────────────────
    bool CanSendGreetings,
    bool CanSendAssociateSpecialOffer,
    bool CanSendOurSpecialOffers,
    bool StatementOnline,
    bool MobileAlert
);

