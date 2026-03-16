using System;
using System.Collections.Generic;
using System.Text;

namespace CO.Application.Contracts.Interfaces.Common;

internal interface IClientNumberGenerator
{
    Task<string> GenerateAsync(CancellationToken ct = default);
}
