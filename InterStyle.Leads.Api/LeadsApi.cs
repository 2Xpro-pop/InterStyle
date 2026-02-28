using InterStyle.Leads.Application.Commands;
using InterStyle.Leads.Application.Queries;
using MediatR;

namespace InterStyle.Leads.Api;

public static class LeadsApi
{
    public static RouteGroupBuilder MapLeadsApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("leads")
            .HasApiVersion(1.0)
            .WithTags("Leads");

        api.MapPost("", async (CreateLeadCommand command, IMediator mediator, CancellationToken ct) =>
        {
            var id = await mediator.Send(command, ct);
            return Results.Ok(new { id = id.Value });
        });

        api.MapGet("statistics", async (
            DateTimeOffset? fromUtc,
            DateTimeOffset? toUtc,
            ILeadQueries queries,
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
