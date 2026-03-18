using Refit;

namespace AdminPanel.Services;

public interface IReviewsApi
{
    [Get("/api/reviews/pending")]
    Task<IReadOnlyList<ReviewDto>> GetPendingAsync();

    [Post("/api/reviews/{id}/approve")]
    Task ApproveAsync(Guid id);

    [Get("/api/reviews/statistics")]
    Task<ReviewStatisticsDto> GetStatisticsAsync(DateTimeOffset? fromUtc = null, DateTimeOffset? toUtc = null);
}

public sealed record ReviewDto(
    Guid Id,
    string CustomerName,
    int Rating,
    string Comment,
    bool IsApproved,
    DateTime CreatedAtUtc,
    DateTime? ApprovedAtUtc);

public sealed record ReviewStatisticsDto(
    long Total,
    decimal AverageRating,
    long Count1,
    long Count2,
    long Count3,
    long Count4,
    long Count5);
