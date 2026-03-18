using InterStyle.Shared;

namespace InterStyle.Curtains.Domain;

/// <summary>
/// Raised when curtain preview URL is changed.
/// </summary>
public sealed record CurtainPreviewChangedDomainEvent(
    CurtainId CurtainId,
    PictureUrl OldPreviewUrl,
    PictureUrl NewPreviewUrl,
    DateTimeOffset OccurredAtUtc) : IDomainEvent;
