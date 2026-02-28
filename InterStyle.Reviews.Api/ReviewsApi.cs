using InterStyle.Reviews.Application.Commands;
using InterStyle.Reviews.Application.Queries;
using MediatR;

namespace InterStyle.Reviews.Api;

public static class ReviewsApi
{
    public static RouteGroupBuilder MapReviewsApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("reviews")
            .HasApiVersion(1.0)
            .WithTags("Reviews");

        api.MapPost("", async (SubmitReviewCommand command, IMediator mediator, CancellationToken ct) =>
        {
            var reviewId = await mediator.Send(command, ct);
            return Results.Created($"/reviews/{reviewId.Value}", new { id = reviewId.Value });
        });

        api.MapPost("{id:guid}/approve", async (Guid id, IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(new ApproveReviewCommand(id), ct);
            return result ? Results.Ok() : Results.NotFound();
        });

        api.MapGet("", async (
            int? page,
            int? pageSize,
            IReviewQueries queries,
            CancellationToken ct) =>
        {
            var result = await queries.GetApprovedAsync(page ?? 1, pageSize ?? 10, ct);
            return Results.Ok(result);
        });

        api.MapGet("pending", async (IReviewQueries queries, CancellationToken ct) =>
        {
            var result = await queries.GetPendingAsync(ct);
            return Results.Ok(result);
        });

        api.MapGet("statistics", async (
            DateTimeOffset? fromUtc,
            DateTimeOffset? toUtc,
            IReviewQueries queries,
            CancellationToken ct) =>
        {
            var now = DateTimeOffset.UtcNow;
            var from = fromUtc ?? now.AddDays(-30);
            var to = toUtc ?? now;

            if (from >= to)
            {
                return Results.BadRequest(new { error = "fromUtc must be earlier than toUtc." });
            }

            var stats = await queries.GetStatisticsAsync(from, to, ct);
            return Results.Ok(stats);
        });

        return api;
    }
}
