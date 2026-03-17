using CO.Domain.Enums;
using CO.Domain.Events;
using CO.Domain.Exceptions;
using CO.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace CO.Domain.Entities;

/// <summary>
/// Client is the aggregate root for corporate client onboarding.
/// All state changes go through this entity — nothing is set directly
/// on owned entities from outside the aggregate.
/// </summary>
public class Client : BaseEntity
{
    // Header Info
    public string ClientNumber { get; private set; } = string.Empty;
    public string ClientName { get; private set; } = string.Empty;
    public ClientType ClientType { get; private set; }
    public SegmentType SegmentType { get; private set; }
    public SubSegmentType SubSegmentType { get; private set; }
    public ClientStatus Status { get; set; }

    // Management
    public DateTimeOffset OpenedOn { get; private set; }
    public Guid RelationshipManagerId { get; private set; }
    public StaffMember? RelationshipManager { get; private set; }

    // Navigation Properties for Tabs
    public CorporateDetail CorporateDetail { get; private set; } = null!;
    public Address Address { get; private set; } = null!;
    public CommunicationPreference CommunicationPreference { get; private set; } = null!;

    private readonly List<Director> _directors = [];
    public IReadOnlyCollection<Director> Directors => _directors.AsReadOnly();

    

    private Client() { }

    /// <summary>
    /// Factory method — the only way to create a valid Client aggregate.
    /// Raises ClientCreatedEvent on success.
    /// </summary>
    public static Client Create(
        string clientNumber,
        string clientName,
        ClientType type,
        SegmentType segmentType,
        SubSegmentType subSegmentType,
        Guid rmId,
        DateTimeOffset openedOn,
        CorporateDetail corporateDetail,
        Address address,
        CommunicationPreference communicationPreference
    )
    {
        return new Client
        {
            Id = Guid.CreateVersion7(),
            ClientNumber = clientNumber,
            ClientName = clientName,
            ClientType = type,
            SegmentType = segmentType,
            SubSegmentType = subSegmentType,
            RelationshipManagerId = rmId,
            OpenedOn = openedOn,
            CorporateDetail = corporateDetail,
            Address = address,
            CommunicationPreference = communicationPreference
        };
    }

    // -------------------------------------------------------------------------
    // Behaviour methods — all state mutations go through here
    // -------------------------------------------------------------------------

    public void SetCorporateDetails(CorporateDetail corporateDetails)
    {
        ArgumentNullException.ThrowIfNull(corporateDetails);
        CorporateDetail = corporateDetails;
    }

    public void SetAddress(Address address)
    {
        ArgumentNullException.ThrowIfNull(address);
        Address = address;
    }

    public void SetCommunicationPreferences(CommunicationPreference preferences)
    {
        ArgumentNullException.ThrowIfNull(preferences);
        CommunicationPreference = preferences;
    }

    public void AssignRelationshipManager(Guid relationshipManagerId)
    {
        if (relationshipManagerId == Guid.Empty)
            throw new DomainException("A Relationship Manager must be assigned.");

        RelationshipManagerId = relationshipManagerId;
    }

    public void AddDirector(Director director)
    {
        ArgumentNullException.ThrowIfNull(director);

        var totalShares = _directors.Sum(d => d.SharePercentage) + director.SharePercentage;
        if (totalShares > 100)
            throw new DomainException(
                $"Total share percentage cannot exceed 100%. Current total would be {totalShares}%.");

        _directors.Add(director);
    }

    public void RemoveDirector(Guid directorId)
    {
        var director = _directors.FirstOrDefault(d => d.Id == directorId)
            ?? throw new DomainException("Director not found.");

        _directors.Remove(director);
    }

    
}
