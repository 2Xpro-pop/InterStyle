using MediatR;

namespace InterStyle.Curtains.Application.Commands;

/// <summary>
/// Command to change a curtain's description.
/// </summary>
/// <param name="CurtainId">The curtain identifier.</param>
/// <param name="NewDescription">The new description for the curtain.</param>
public sealed record ChangeCurtainDescriptionCommand(
    Guid CurtainId,
    string NewDescription) : IRequest;
