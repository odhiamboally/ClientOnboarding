using CO.Shared.Dtos.Client;
using CO.Shared.Dtos.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace CO.Application.Contracts.Interfaces.Services;

public interface ILookupService
{
    List<LookupResponse> GetClientTypes();
    List<LookupResponse> GetSegmentTypes();
    List<LookupResponse> GetSubSegmentTypes();
    List<LookupResponse> GetLinesOfBusiness();
    List<LookupResponse> GetIdentificationTypes();
    List<LookupResponse> GetLookup<T>() where T : struct, Enum;
    Task<List<StaffMemberResponse>> GetRelationshipManagers(string userId);

}
