using CO.Shared.Dtos.Common;

namespace CO.UI.Blazor.Features.Utilities;

public class LookupBundle
{
    public List<LookupResponse> ClientTypes { get; set; } = [];
    public List<LookupResponse> SegmentTypes { get; set; } = [];
    public List<LookupResponse> SubSegmentTypes { get; set; } = [];
    public List<LookupResponse> IdentificationTypes { get; set; } = [];
    public List<LookupResponse> LinesOfBusiness { get; set; } = [];
    public List<LookupResponse> Statuses { get; set; } = [];
}
