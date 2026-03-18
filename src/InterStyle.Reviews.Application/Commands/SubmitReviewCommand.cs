using InterStyle.Reviews.Domain;
using MediatR;

namespace InterStyle.Reviews.Application.Commands;

/// <summary>
/// Command to submit a new customer review.
/// </summary>
/// <param name="CustomerName">The customer's name.</param>
/// <param name="Rating">The rating from 1 to 5.</param>
/// <param name="Comment">The review comment text (5-2000 characters).</param>
public sealed record SubmitReviewCommand(
    string CustomerName,
    int Rating,
    string Comment) : IRequest<ReviewId>;
