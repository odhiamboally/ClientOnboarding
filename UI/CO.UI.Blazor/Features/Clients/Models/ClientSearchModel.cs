using CO.Shared.Dtos.Client;

namespace CO.UI.Blazor.Features.Clients.Models;

public class ClientSearchModel
{
    public string? GlobalSearch { get; set; }
    public string? ClientType { get; set; }
    public string? SegmentType { get; set; }
    public string? SubSegmentType { get; set; }
    public string? IdentificationType { get; set; }
    public string? LineOfBusiness { get; set; }
    public string? Status { get; set; }
    public Guid? RelationshipManagerId { get; set; }
    public Guid? Cursor { get; set; }
    public int PageSize { get; set; } = 50;

    public ClientSearchRequest ToRequest() => new(
        GlobalSearch,
        ClientType,
        SegmentType,
        SubSegmentType,
        IdentificationType,
        LineOfBusiness,
        Status,
        RelationshipManagerId,
        Cursor,
        PageSize
    );

    public void Reset()
    {
        GlobalSearch = null;
        ClientType = null;
        SegmentType = null;
        SubSegmentType = null;
        IdentificationType = null;
        LineOfBusiness = null;
        Status = null;
        RelationshipManagerId = null;
        Cursor = null;
    }
}
