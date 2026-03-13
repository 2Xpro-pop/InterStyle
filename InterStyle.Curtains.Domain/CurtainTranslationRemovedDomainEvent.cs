using InterStyle.Shared;

namespace InterStyle.Curtains.Domain;

/// <summary>
/// Raised when a translation is removed from a curtain.
/// </summary>
public sealed record CurtainTranslationRemovedDomainEvent(
    CurtainId CurtainId,
    string Locale,
    DateTimeOffset OccurredAtUtc) : IDomainEvent;
