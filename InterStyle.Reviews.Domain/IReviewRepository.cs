using InterStyle.Shared;

namespace InterStyle.Reviews.Domain;

/// <summary>
/// Repository contract for Review aggregate.
/// </summary>
public interface IReviewRepository : IRepository<Review>
{
    /// <summary>
    /// Gets a review by its identifier.
    /// </summary>
    /// <param name="id">The review identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The review if found; otherwise null.</returns>
    Task<Review?> GetByIdAsync(ReviewId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new review to the repository.
    /// </summary>
    /// <param name="review">The review to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task AddAsync(Review review, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing review in the repository.
    /// </summary>
    /// <param name="review">The review to update.</param>
    void Update(Review review);
}
