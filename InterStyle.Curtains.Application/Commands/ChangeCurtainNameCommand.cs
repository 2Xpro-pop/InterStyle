using MediatR;

namespace InterStyle.Curtains.Application.Commands;

/// <summary>
/// Command to change a curtain's name.
/// </summary>
/// <param name="CurtainId">The curtain identifier.</param>
/// <param name="NewName">The new name for the curtain.</param>
public sealed record ChangeCurtainNameCommand(
    Guid CurtainId,
    string NewName) : IRequest;
