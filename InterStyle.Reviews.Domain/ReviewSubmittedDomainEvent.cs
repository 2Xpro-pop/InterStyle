using InterStyle.Shared;

namespace InterStyle.Reviews.Domain;

/// <summary>
/// Raised when a new review is submitted by a customer.
/// </summary>
public sealed record ReviewSubmittedDomainEvent(
    ReviewId ReviewId,
    DateTimeOffset OccurredAtUtc) : IDomainEvent;
