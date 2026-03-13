using InterStyle.Shared;

namespace InterStyle.Curtains.Domain;

/// <summary>
/// Raised when an existing translation of a curtain is updated.
/// </summary>
public sealed record CurtainTranslationUpdatedDomainEvent(
    CurtainId CurtainId,
    string Locale,
    DateTimeOffset OccurredAtUtc) : IDomainEvent;
