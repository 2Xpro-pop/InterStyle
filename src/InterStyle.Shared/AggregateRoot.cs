using InterStyle.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace InterStyle.Shared;

/// <summary>
/// Base class for aggregate roots.
/// </summary>
public abstract class AggregateRoot<TId> : Entity<TId>, IAggregateRoot where TId : notnull
{
    private readonly List<IDomainEvent> domainEvents = [];

    protected AggregateRoot()
    {
    }

    protected AggregateRoot(TId id) : base(id)
    {
    }

    public IReadOnlyCollection<IDomainEvent> DomainEvents => domainEvents;

    protected void AddDomainEvent(IDomainEvent domainEvent)
        => domainEvents.Add(domainEvent);

    public void ClearDomainEvents() => domainEvents.Clear();
}