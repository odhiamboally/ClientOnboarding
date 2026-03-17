using CO.Application.Contracts.Interfaces.Services;
using CO.Application.Extensions;
using CO.Application.Features.StaffMembers.Queries;
using CO.Application.Mappings;
using CO.Domain.Entities;
using CO.Domain.Enums;
using CO.Shared.Dtos.Client;
using CO.Shared.Dtos.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CO.Application.Contracts.Implementations.Services;

internal sealed class LookupService(ISender sender) : ILookupService
{
    private List<StaffMemberResponse>? _staffCache;

    public List<LookupResponse> GetClientTypes() => [.. EnumExtensions.ToLookupResponses<ClientType>()];
    public List<LookupResponse> GetSegmentTypes() => [.. EnumExtensions.ToLookupResponses<SegmentType>()];
    public List<LookupResponse> GetSubSegmentTypes() => [.. EnumExtensions.ToLookupResponses<SubSegmentType>()];
    public List<LookupResponse> GetLinesOfBusiness() => [.. EnumExtensions.ToLookupResponses<LineOfBusiness>()];
    public List<LookupResponse> GetIdentificationTypes() => [.. EnumExtensions.ToLookupResponses<IdentificationType>()];

    public List<LookupResponse> GetLookup<T>() where T : struct, Enum
    {
        return [.. EnumExtensions.ToLookupResponses<T>()];
    }

    public async Task<List<StaffMemberResponse>> GetRelationshipManagers(string userId)
    {
        if (_staffCache != null) return _staffCache;

        var result = await sender.Send(new GetStaffMembersQuery(userId));
        if (result.Successful)
        {
            _staffCache = result.Data ?? [];
        }

        return _staffCache ?? [];
    }

}
