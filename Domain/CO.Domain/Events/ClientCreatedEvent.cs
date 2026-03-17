using CO.Domain.Contracts.Interfaces.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace CO.Domain.Events;

public sealed class ClientCreatedEvent() : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}

