using InterStyle.Reviews.Api.Services;
using InterStyle.Reviews.Application.Commands;
using InterStyle.Reviews.Application.Queries;
using InterStyle.Reviews.Domain;
using MediatR;

namespace InterStyle.Reviews.Api;

public static class ReviewsApi
{
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
            var logger = loggerFactory.CreateLogger("ReviewsApi");

            if (!await captchaValidator.ValidateAsync(request.CaptchaToken, ct))
            {
                logger.LogWarning("Invalid captcha token for review submission by {CustomerName}", request.CustomerName);
                return Results.BadRequest(new { error = "Invalid captcha token." });
            }

            logger.LogInformation("Submitting review from {CustomerName} with rating {Rating}", request.CustomerName, request.Rating);

            var command = new SubmitReviewCommand(request.CustomerName, request.Rating, request.Comment);
            var reviewId = await mediator.Send(command, ct);

            logger.LogInformation("Review {ReviewId} submitted successfully", reviewId.Value);

            return Results.Created($"/reviews/{reviewId.Value}", new { id = reviewId.Value });
        });

        api.MapPost("{id:guid}/approve", async (Guid id, IMediator mediator, ILoggerFactory loggerFactory, CancellationToken ct) =>
        {
            var logger = loggerFactory.CreateLogger("ReviewsApi");

            logger.LogInformation("Approving review {ReviewId}", id);

            var result = await mediator.Send(new ApproveReviewCommand(id), ct);

            if (!result)
            {
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
            var logger = loggerFactory.CreateLogger("ReviewsApi");

            logger.LogInformation("Retrieving approved reviews, page {Page}, pageSize {PageSize}", page ?? 1, pageSize ?? 10);

            var result = await queries.GetApprovedAsync(page ?? 1, pageSize ?? 10, ct);
            return Results.Ok(result);
        });

        api.MapGet("pending", async (IReviewQueries queries, ILoggerFactory loggerFactory, CancellationToken ct) =>
        {
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
            var logger = loggerFactory.CreateLogger("ReviewsApi");

            var now = DateTimeOffset.UtcNow;
            var from = fromUtc ?? now.AddDays(-30);
            var to = toUtc ?? now;

            if (from >= to)
            {
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