using InterStyle.ApiShared;
using InterStyle.Reviews.Application.Commands;
using InterStyle.Reviews.Application.Queries;
using InterStyle.Reviews.Domain;
using InterStyle.Reviews.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<ApproveReviewCommand>());

builder.Services.AddDefaultDbContext<ReviewsDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("reviewsdb"));
});

var withApiVersioning = builder.Services.AddApiVersioning();

builder.AddDefaultOpenApi(withApiVersioning);

builder.Services.AddScoped<IReviewRepository, ReviewRepository>();

builder.Services.AddScoped<IReviewQueries, ReviewQueries>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwaggerDefaults();
app.UseHealthChecksDefaults();

app.UseDefaultOpenApi();

app.MapPost("/reviews", async (SubmitReviewCommand command, IMediator mediator, CancellationToken ct) =>
{
    var reviewId = await mediator.Send(command, ct);
    return Results.Created($"/reviews/{reviewId.Value}", new { id = reviewId.Value });
});

app.MapPost("/reviews/{id:guid}/approve", async (Guid id, IMediator mediator, CancellationToken ct) =>
{
    var result = await mediator.Send(new ApproveReviewCommand(id), ct);
    return result ? Results.Ok() : Results.NotFound();
});

app.MapGet("/reviews", async (
    int? page,
    int? pageSize,
    IReviewQueries queries,
    CancellationToken ct) =>
{
    var result = await queries.GetApprovedAsync(page ?? 1, pageSize ?? 10, ct);
    return Results.Ok(result);
});

app.MapGet("/reviews/pending", async (IReviewQueries queries, CancellationToken ct) =>
{
    var result = await queries.GetPendingAsync(ct);
    return Results.Ok(result);
});

app.MapGet("/reviews/statistics", async (
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

app.Run();
