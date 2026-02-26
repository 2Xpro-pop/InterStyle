using InterStyle.Curtains.Domain;
using MediatR;

namespace InterStyle.Curtains.Application.Commands;

/// <summary>
/// Command to create a new curtain.
/// </summary>
/// <param name="Name">The curtain name.</param>
/// <param name="Description">The curtain description.</param>
/// <param name="PictureUrl">The URL to the full picture.</param>
/// <param name="PreviewUrl">The URL to the preview picture.</param>
public sealed record CreateCurtainCommand(
    string Name,
    string Description,
    string PictureUrl,
    string PreviewUrl) : IRequest<CurtainId>;
