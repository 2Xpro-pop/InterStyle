using InterStyle.Curtains.Domain;

namespace InterStyle.Curtains.Application.Queries;

/// <summary>
/// Query interface for read-side curtain operations (CQRS).
/// Implemented using Dapper for optimal read performance.
/// </summary>
public interface ICurtainQueries
{
    /// <summary>
    /// Gets all curtains with translations for the specified locale.
    /// Fallback order: requested locale → default locale → any available translation.
    /// </summary>
    /// <param name="locale">The preferred locale.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of all curtains.</returns>
    Task<IReadOnlyList<CurtainDto>> GetAllAsync(Locale locale, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a curtain by its identifier with translations for the specified locale.
    /// Fallback order: requested locale → default locale → any available translation.
    /// </summary>
    /// <param name="id">The curtain identifier.</param>
    /// <param name="locale">The preferred locale.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The curtain if found; otherwise null.</returns>
    Task<CurtainDto?> GetByIdAsync(Guid id, Locale locale, CancellationToken cancellationToken = default);
}

/// <summary>
/// DTO representing a curtain for read operations.
/// </summary>
public sealed record CurtainDto(
    Guid Id,
    string Name,
    string Description,
    string Locale,
    string PictureUrl,
    string PreviewUrl);

/// <summary>
/// DTO representing a single curtain translation.
/// </summary>
public sealed record CurtainTranslationDto(
    string Locale,
    string Name,
    string Description);
