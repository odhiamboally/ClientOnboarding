using CO.Domain.Contracts.Specifications;
using CO.Domain.Entities;
using CO.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace CO.Domain.Contracts.Implementations.Specifications;

public class ClientSearchSpec : Specification<Client, Guid>, IClientSearchSpec
{
    public ClientSearchSpec(
        string? globalSearch,
        ClientType? clientType,
        SegmentType? segmentType,
        SubSegmentType? subSegmentType,
        IdentificationType? identificationType,
        LineOfBusiness? lineOfBusiness,
        ClientStatus? status,
        Guid? relationshipManagerId,
        Guid? cursor,
        int pageSize
        
    )
    {
        AddCriteria(client =>
            (string.IsNullOrWhiteSpace(globalSearch) ||
                client.ClientNumber.Contains(globalSearch) ||
                client.CorporateDetail.CompanyName.Contains(globalSearch) ||
                client.CorporateDetail.RegistrationNumber.Contains(globalSearch) ||
                (client.CorporateDetail.TINNumber != null && client.CorporateDetail.TINNumber.Contains(globalSearch)) ||
                (client.Address.Mobile != null && client.Address.Mobile.Contains(globalSearch)) ||
                (client.Address.EmailId != null && client.Address.EmailId.Contains(globalSearch)))

            && (!clientType.HasValue || client.ClientType == clientType.Value)
            && (!segmentType.HasValue || client.SegmentType == segmentType.Value)
            && (!subSegmentType.HasValue || client.SubSegmentType == subSegmentType.Value)
            && (!identificationType.HasValue || client.CorporateDetail.IdentificationType == identificationType.Value)
            && (!lineOfBusiness.HasValue || client.CorporateDetail.LineOfBusiness == lineOfBusiness.Value)
            && (!status.HasValue || client.Status == status.Value)
            && (!relationshipManagerId.HasValue || client.RelationshipManagerId == relationshipManagerId.Value)
        );

        AddInclude(c => c.RelationshipManager!);
        AddOrderBy(c => c.Id);

        // Only set the cursor if it actually exists. If null, the Evaluator knows to start from the beginning.
        if (cursor.HasValue && cursor.Value != Guid.Empty)
        {
            SetCursor(cursor.Value, c => c.Id > cursor.Value);
            //SetCursor(cursor.Value, "Id");
        }

        SetTake(Math.Clamp(pageSize, 1, 50)); // defensive — spec shouldn't trust its caller
        EnableSplitQuery();
    }

}
