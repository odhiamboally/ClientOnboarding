using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CO.Domain.Contracts.Interfaces.Common;

public interface IDomainEvent : INotification
{
    DateTimeOffset OccurredOn { get; }
}
