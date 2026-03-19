using InterStyle.Reviews.Api.Services;
using InterStyle.Reviews.Application.Commands;
using InterStyle.Reviews.Application.Queries;
using InterStyle.Reviews.Domain;
using MediatR;
using System.Diagnostics;

namespace InterStyle.Reviews.Api;

public static class ReviewsApi
{
    private static readonly ActivitySource ActivitySource = new("InterStyle.Reviews.Api");

    public static RouteGroupBuilder MapReviewsApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/reviews")
            .HasApiVersion(1.0)
            .WithTags("Reviews");

        api.MapPost("", async (
            SubmitReviewViewModel request, 
            IMediator mediator, 
            ICaptchaValidator captchaValidator,
            ILoggerFactory loggerFactory,
            CancellationToken ct) =>
        {
            using var activity = ActivitySource.StartActivity("SubmitReview");
            var logger = loggerFactory.CreateLogger("ReviewsApi");

            if (!await captchaValidator.ValidateAsync(request.CaptchaToken, ct))
            {
                activity?.SetStatus(ActivityStatusCode.Error, "Invalid captcha token");
                logger.LogWarning("Invalid captcha token for review submission by {CustomerName}", request.CustomerName);
                return Results.BadRequest(new { error = "Invalid captcha token." });
            }

            activity?.SetTag("review.customer", request.CustomerName);
            activity?.SetTag("review.rating", request.Rating);
            logger.LogInformation("Submitting review from {CustomerName} with rating {Rating}", request.CustomerName, request.Rating);

            var command = new SubmitReviewCommand(request.CustomerName, request.Rating, request.Comment);
            var reviewId = await mediator.Send(command, ct);

            activity?.SetTag("review.id", reviewId.Value);
            logger.LogInformation("Review {ReviewId} submitted successfully", reviewId.Value);

            return Results.Created($"/reviews/{reviewId.Value}", new { id = reviewId.Value });
        });

        api.MapPost("{id:guid}/approve", async (Guid id, IMediator mediator, ILoggerFactory loggerFactory, CancellationToken ct) =>
        {
            using var activity = ActivitySource.StartActivity("ApproveReview");
            var logger = loggerFactory.CreateLogger("ReviewsApi");

            activity?.SetTag("review.id", id);
            logger.LogInformation("Approving review {ReviewId}", id);

            var result = await mediator.Send(new ApproveReviewCommand(id), ct);

            if (!result)
            {
                activity?.SetStatus(ActivityStatusCode.Error, "Review not found");
                logger.LogWarning("Review {ReviewId} not found for approval", id);
                return Results.NotFound();
            }

            logger.LogInformation("Review {ReviewId} approved successfully", id);

            return Results.Ok();
        });

        api.MapGet("", async (
            int? page,
            int? pageSize,
            IReviewQueries queries,
            ILoggerFactory loggerFactory,
            CancellationToken ct) =>
        {
            using var activity = ActivitySource.StartActivity("GetApprovedReviews");
            var logger = loggerFactory.CreateLogger("ReviewsApi");

            activity?.SetTag("reviews.page", page ?? 1);
            activity?.SetTag("reviews.pageSize", pageSize ?? 10);
            logger.LogInformation("Retrieving approved reviews, page {Page}, pageSize {PageSize}", page ?? 1, pageSize ?? 10);

            var result = await queries.GetApprovedAsync(page ?? 1, pageSize ?? 10, ct);
            return Results.Ok(result);
        });

        api.MapGet("pending", async (IReviewQueries queries, ILoggerFactory loggerFactory, CancellationToken ct) =>
        {
            using var activity = ActivitySource.StartActivity("GetPendingReviews");
            var logger = loggerFactory.CreateLogger("ReviewsApi");

            logger.LogInformation("Retrieving pending reviews");

            var result = await queries.GetPendingAsync(ct);
            return Results.Ok(result);
        });

        api.MapGet("statistics", async (
            DateTimeOffset? fromUtc,
            DateTimeOffset? toUtc,
            IReviewQueries queries,
            ILoggerFactory loggerFactory,
            CancellationToken ct) =>
        {
            using var activity = ActivitySource.StartActivity("GetReviewStatistics");
            var logger = loggerFactory.CreateLogger("ReviewsApi");

            var now = DateTimeOffset.UtcNow;
            var from = fromUtc ?? now.AddDays(-30);
            var to = toUtc ?? now;

            activity?.SetTag("statistics.from", from.ToString("o"));
            activity?.SetTag("statistics.to", to.ToString("o"));

            if (from >= to)
            {
                activity?.SetStatus(ActivityStatusCode.Error, "Invalid date range");
                logger.LogWarning("Invalid date range for review statistics: {FromUtc} >= {ToUtc}", from, to);
                return Results.BadRequest(new { error = "fromUtc must be earlier than toUtc." });
            }

            logger.LogInformation("Retrieving review statistics from {FromUtc} to {ToUtc}", from, to);

            var stats = await queries.GetStatisticsAsync(from, to, ct);
            return Results.Ok(stats);
        });

        return api;
    }
}

file sealed record class SubmitReviewViewModel(
    string CustomerName,
    int Rating,
    string Comment,
    string CaptchaToken);