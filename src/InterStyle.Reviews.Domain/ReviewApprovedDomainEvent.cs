using InterStyle.Shared;

namespace InterStyle.Reviews.Domain;

/// <summary>
/// Raised when a review is approved by an administrator.
/// </summary>
public sealed record ReviewApprovedDomainEvent(
    ReviewId ReviewId,
    DateTimeOffset OccurredAtUtc) : IDomainEvent;
