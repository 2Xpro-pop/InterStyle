using InterStyle.Shared;

namespace InterStyle.Curtains.Domain;

/// <summary>
/// Raised when curtain name is changed.
/// </summary>
public sealed record CurtainNameChangedDomainEvent(
    CurtainId CurtainId,
    CurtainName OldName,
    CurtainName NewName,
    DateTimeOffset OccurredAtUtc) : IDomainEvent;
