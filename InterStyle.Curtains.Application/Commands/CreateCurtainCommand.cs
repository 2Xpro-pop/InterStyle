using InterStyle.Curtains.Domain;
using MediatR;

namespace InterStyle.Curtains.Application.Commands;

/// <summary>
/// Command to create a new curtain with an initial translation.
/// </summary>
/// <param name="Locale">The locale for the initial translation (BCP 47 tag).</param>
/// <param name="Name">The curtain name in the given locale.</param>
/// <param name="Description">The curtain description in the given locale.</param>
/// <param name="PictureUrl">The URL to the full picture.</param>
/// <param name="PreviewUrl">The URL to the preview picture.</param>
public sealed record CreateCurtainCommand(
    string Locale,
    string Name,
    string Description,
    string PictureUrl,
    string PreviewUrl) : IRequest<CurtainId>;
