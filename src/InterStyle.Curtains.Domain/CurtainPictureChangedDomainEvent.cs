using InterStyle.Shared;

namespace InterStyle.Curtains.Domain;

/// <summary>
/// Raised when curtain picture URL is changed.
/// </summary>
public sealed record CurtainPictureChangedDomainEvent(
    CurtainId CurtainId,
    PictureUrl OldPictureUrl,
    PictureUrl NewPictureUrl,
    DateTimeOffset OccurredAtUtc) : IDomainEvent;
