using InterStyle.Leads.Domain;
using InterStyle.Shared;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace InterStyle.Leads.Infrastructure.Persistence;

static class MediatorExtension
{
    public static async Task DispatchDomainEventsAsync(this IMediator mediator, LeadsDbContext context)
    {
        var domainEntities = context.ChangeTracker
            .Entries<Lead>()
            .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Count != 0);

        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.DomainEvents)
            .ToList();

        domainEntities.ToList()
            .ForEach(entity => entity.Entity.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
            await mediator.Publish(domainEvent);
    }
}