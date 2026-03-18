using MediatR;

namespace InterStyle.Curtains.Application.Commands;

/// <summary>
/// Command to change a curtain's preview URL.
/// </summary>
/// <param name="CurtainId">The curtain identifier.</param>
/// <param name="NewPreviewUrl">The new preview URL for the curtain.</param>
public sealed record ChangeCurtainPreviewCommand(
    Guid CurtainId,
    string NewPreviewUrl) : IRequest;
