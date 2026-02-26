using InterStyle.Shared;

namespace InterStyle.Curtains.Domain;

/// <summary>
/// Raised when curtain description is changed.
/// </summary>
public sealed record CurtainDescriptionChangedDomainEvent(
    CurtainId CurtainId,
    Description OldDescription,
    Description NewDescription,
    DateTimeOffset OccurredAtUtc) : IDomainEvent;
