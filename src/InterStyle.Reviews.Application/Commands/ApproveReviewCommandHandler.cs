using InterStyle.Reviews.Domain;
using MediatR;

namespace InterStyle.Reviews.Application.Commands;

/// <summary>
/// Handler for <see cref="ApproveReviewCommand"/>.
/// </summary>
public sealed class ApproveReviewCommandHandler(
    IReviewRepository reviewRepository,
    IReviewCacheInvalidator cacheInvalidator)
    : IRequestHandler<ApproveReviewCommand, bool>
{
    private readonly IReviewRepository _reviewRepository = reviewRepository;
    private readonly IReviewCacheInvalidator _cacheInvalidator = cacheInvalidator;

    /// <inheritdoc />
    public async Task<bool> Handle(ApproveReviewCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var reviewId = new ReviewId(request.ReviewId);
        var review = await _reviewRepository.GetByIdAsync(reviewId, cancellationToken);

        if (review is null)
        {
            return false;
        }

        review.Approve(DateTimeOffset.UtcNow);

        _reviewRepository.Update(review);
        await _reviewRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        await _cacheInvalidator.InvalidateApprovedReviewsAsync(cancellationToken);

        return true;
    }
}
