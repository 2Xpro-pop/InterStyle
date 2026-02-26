using MediatR;

namespace InterStyle.Curtains.Application.Commands;

/// <summary>
/// Command to change a curtain's picture URL.
/// </summary>
/// <param name="CurtainId">The curtain identifier.</param>
/// <param name="NewPictureUrl">The new picture URL for the curtain.</param>
public sealed record ChangeCurtainPictureCommand(
    Guid CurtainId,
    string NewPictureUrl) : IRequest;
