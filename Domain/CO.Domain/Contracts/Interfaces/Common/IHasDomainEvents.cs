using System;
using System.Collections.Generic;
using System.Text;

namespace CO.Domain.Contracts.Interfaces.Common;

public interface IHasDomainEvents
{
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
    void ClearDomainEvents();
}
