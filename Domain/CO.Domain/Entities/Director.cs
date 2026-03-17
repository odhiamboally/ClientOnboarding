using CO.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CO.Domain.Entities;

/// <summary>
/// Child entity of Client. Has its own identity — can be added/removed independently.
/// Corresponds to Directors' Details tab in the original UI.
/// </summary>
public class Director : BaseEntity
{
    public Guid ClientId { get; private set; }
    public string FullName { get; private set; } = string.Empty;
    public DirectorRelationType RelationType { get; private set; }
    public IdentificationType IdentificationType { get; private set; }
    public string IdentificationNumber { get; private set; } = string.Empty;
    public string? PhoneNumber { get; private set; }
    public string? Email { get; private set; }
    public decimal? SharePercentage { get; private set; }
    public DateTime DateAdded { get; private set; }

    // EF Core
    private Director() { }

    public static Director Create(
        Guid clientId,
        string fullName,
        DirectorRelationType relationType,
        IdentificationType identificationType,
        string identificationNumber,
        string? phoneNumber = null,
        string? email = null,
        decimal? sharePercentage = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fullName);
        ArgumentException.ThrowIfNullOrWhiteSpace(identificationNumber);

        if (sharePercentage is < 0 or > 100)
            throw new ArgumentOutOfRangeException(nameof(sharePercentage), "Share percentage must be between 0 and 100.");

        return new Director
        {
            Id = Guid.NewGuid(),
            ClientId = clientId,
            FullName = fullName.Trim(),
            RelationType = relationType,
            IdentificationType = identificationType,
            IdentificationNumber = identificationNumber.Trim().ToUpper(),
            PhoneNumber = phoneNumber?.Trim(),
            Email = email?.Trim().ToLower(),
            SharePercentage = sharePercentage,
            DateAdded = DateTime.UtcNow
        };
    }

    internal void Update(
        string fullName,
        DirectorRelationType relationType,
        IdentificationType identificationType,
        string identificationNumber,
        string? phoneNumber = null,
        string? email = null,
        decimal? sharePercentage = null)
    {
        FullName = fullName.Trim();
        RelationType = relationType;
        IdentificationType = identificationType;
        IdentificationNumber = identificationNumber.Trim().ToUpper();
        PhoneNumber = phoneNumber?.Trim();
        Email = email?.Trim().ToLower();
        SharePercentage = sharePercentage;
    }
}
