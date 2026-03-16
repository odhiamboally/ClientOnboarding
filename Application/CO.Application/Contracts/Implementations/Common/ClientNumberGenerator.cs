using CO.Application.Contracts.Interfaces.Common;
using CO.Domain.Contracts.Interfaces.Common;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace CO.Application.Contracts.Implementations.Common;

internal sealed class ClientNumberGenerator(IUnitOfWork _unitOfWork) : IClientNumberGenerator
{
    public async Task<string> GenerateAsync(CancellationToken ct = default)
    {
        var prefix = "CLT";
        var totalCount = await _unitOfWork.ClientRepository.CountAsync(ct);
        var sequence = totalCount + 1;
        return $"{prefix}-{sequence:D5}"; // CLT-00001
    }
}

