using InterStyle.Reviews.Domain;
using MediatR;

namespace InterStyle.Reviews.Infrastructure.Persistence;

/// <summary>
/// Extension methods for dispatching domain events via MediatR.
/// </summary>
internal static class MediatorExtension
{
    /// <summary>
    /// Dispatches all domain events from tracked Review entities.
    /// </summary>
    /// <param name="mediator">The mediator instance.</param>
    /// <param name="context">The DbContext containing tracked entities.</param>
    public static async Task DispatchDomainEventsAsync(this IMediator mediator, ReviewsDbContext context)
    {
        var domainEntities = context.ChangeTracker
            .Entries<Review>()
            .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Count != 0)
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.DomainEvents)
            .ToList();

        domainEntities.ForEach(entity => entity.Entity.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
        {
            await mediator.Publish(domainEvent);
        }
    }
}
