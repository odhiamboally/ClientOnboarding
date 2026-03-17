using System;
using System.Collections.Generic;
using System.Text;

namespace CO.Shared.Dtos.Client;

public record StaffMemberResponse(
    Guid Id,
    string FullName,
    string StaffCode,
    string? Department
);
