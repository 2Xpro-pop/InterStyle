namespace InterStyle.Curtains.Application.Queries;

/// <summary>
/// Query interface for read-side curtain operations (CQRS).
/// Implemented using Dapper for optimal read performance.
/// </summary>
public interface ICurtainQueries
{
    /// <summary>
    /// Gets all curtains.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of all curtains.</returns>
    Task<IReadOnlyList<CurtainDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a curtain by its identifier.
    /// </summary>
    /// <param name="id">The curtain identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The curtain if found; otherwise null.</returns>
    Task<CurtainDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}

/// <summary>
/// DTO representing a curtain for read operations.
/// </summary>
public sealed record CurtainDto(
    Guid Id,
    string Name,
    string Description,
    string PictureUrl,
    string PreviewUrl);
