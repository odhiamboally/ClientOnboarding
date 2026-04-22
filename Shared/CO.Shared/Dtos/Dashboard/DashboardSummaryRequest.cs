using System;
using System.Collections.Generic;
using System.Text;

namespace CO.Shared.Dtos.Dashboard;

public record DashboardSummaryRequest(string UserId, string? RoleScope = null);
