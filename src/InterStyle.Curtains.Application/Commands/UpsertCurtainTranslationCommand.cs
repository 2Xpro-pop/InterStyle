using MediatR;

namespace InterStyle.Curtains.Application.Commands;

/// <summary>
/// Command to add or update a translation for a curtain.
/// </summary>
/// <param name="CurtainId">The curtain identifier.</param>
/// <param name="Locale">The locale for the translation (BCP 47 tag).</param>
/// <param name="Name">The curtain name in the given locale.</param>
/// <param name="Description">The curtain description in the given locale.</param>
public sealed record UpsertCurtainTranslationCommand(
    Guid CurtainId,
    string Locale,
    string Name,
    string Description) : IRequest;
