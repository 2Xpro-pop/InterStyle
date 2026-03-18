using InterStyle.Shared;

namespace InterStyle.Curtains.Domain;

/// <summary>
/// Raised when a new translation is added to a curtain.
/// </summary>
public sealed record CurtainTranslationAddedDomainEvent(
    CurtainId CurtainId,
    string Locale,
    DateTimeOffset OccurredAtUtc) : IDomainEvent;
