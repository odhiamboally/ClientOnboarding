using System;
using System.Collections.Generic;
using System.Text;

namespace CO.Shared.Dtos.Common;

public record LookupResponse(int Id, string Name, string? Description);