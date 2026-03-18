namespace InterStyle.Reviews.Application;

/// <summary>
/// Invalidates cached review query results.
/// </summary>
public interface IReviewCacheInvalidator
{
    Task InvalidateApprovedReviewsAsync(CancellationToken cancellationToken = default);
}
