using InterStyle.Reviews.Domain;
using MediatR;

namespace InterStyle.Reviews.Application.Commands;

/// <summary>
/// Handler for <see cref="SubmitReviewCommand"/>.
/// </summary>
public sealed class SubmitReviewCommandHandler(IReviewRepository reviewRepository)
    : IRequestHandler<SubmitReviewCommand, ReviewId>
{
    private readonly IReviewRepository _reviewRepository = reviewRepository;

    /// <inheritdoc />
    public async Task<ReviewId> Handle(SubmitReviewCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var customerName = CustomerName.Create(request.CustomerName);
        var rating = Rating.Create(request.Rating);
        var comment = ReviewComment.Create(request.Comment);

        var review = Review.Submit(
            customerName: customerName,
            rating: rating,
            comment: comment,
            createdAtUtc: DateTimeOffset.UtcNow);

        await _reviewRepository.AddAsync(review, cancellationToken);
        await _reviewRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return review.Id;
    }
}
