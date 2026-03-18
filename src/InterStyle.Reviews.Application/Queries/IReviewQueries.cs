namespace InterStyle.Reviews.Application.Queries;

/// <summary>
/// Query interface for read-side review operations (CQRS).
/// Implemented using Dapper for optimal read performance.
/// </summary>
public interface IReviewQueries
{
    /// <summary>
    /// Gets a paginated list of approved reviews.
    /// </summary>
    /// <param name="page">Page number (1-based).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paginated list of approved reviews.</returns>
    Task<PagedResult<ReviewDto>> GetApprovedAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all pending (unapproved) reviews.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of pending reviews.</returns>
    Task<IReadOnlyList<ReviewDto>> GetPendingAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets review statistics for a specified time period.
    /// </summary>
    /// <param name="fromUtc">Start of the period (inclusive).</param>
    /// <param name="toUtc">End of the period (exclusive).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Review statistics for the period.</returns>
    Task<ReviewStatistics> GetStatisticsAsync(
        DateTimeOffset fromUtc,
        DateTimeOffset toUtc,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// DTO representing a review for read operations.
/// </summary>
public sealed record ReviewDto(
    Guid Id,
    string CustomerName,
    int Rating,
    string Comment,
    bool IsApproved,
    DateTime CreatedAtUtc,
    DateTime? ApprovedAtUtc);

/// <summary>
/// DTO containing review statistics for a time period.
/// </summary>
public sealed record ReviewStatistics(
    long Total,
    decimal AverageRating,
    long Count1,
    long Count2,
    long Count3,
    long Count4,
    long Count5);

/// <summary>
/// Generic paginated result wrapper.
/// </summary>
/// <typeparam name="T">Type of items in the result.</typeparam>
public sealed record PagedResult<T>(
    IReadOnlyList<T> Items,
    int Page,
    int PageSize,
    long TotalCount)
{
    /// <summary>
    /// Gets the total number of pages.
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// Gets a value indicating whether there is a next page.
    /// </summary>
    public bool HasNextPage => Page < TotalPages;

    /// <summary>
    /// Gets a value indicating whether there is a previous page.
    /// </summary>
    public bool HasPreviousPage => Page > 1;
}
