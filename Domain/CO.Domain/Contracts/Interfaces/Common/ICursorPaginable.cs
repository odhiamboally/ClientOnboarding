using System;
using System.Collections.Generic;
using System.Text;

namespace CO.Domain.Contracts.Interfaces.Common;

public interface ICursorPaginable
{
    Guid Id { get; }
    DateTimeOffset CreatedAt { get; }
}

