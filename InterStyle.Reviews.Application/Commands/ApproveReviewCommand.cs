using MediatR;

namespace InterStyle.Reviews.Application.Commands;

/// <summary>
/// Command to approve a pending review.
/// </summary>
/// <param name="ReviewId">The identifier of the review to approve.</param>
public sealed record ApproveReviewCommand(Guid ReviewId) : IRequest<bool>;
