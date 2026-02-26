using System;
using System.Collections.Generic;
using System.Text;

namespace InterStyle.Shared;

public interface IAggregateRoot
{
    public IReadOnlyCollection<IDomainEvent> DomainEvents
    {
        get;
    }

    public void ClearDomainEvents();
}
