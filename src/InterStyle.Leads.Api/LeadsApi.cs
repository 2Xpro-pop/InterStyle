using InterStyle.ApiShared;
using InterStyle.ApiShared.Auth;
using InterStyle.Leads.Application.Commands;
using InterStyle.Leads.Application.Queries;
using MediatR;
using System.Diagnostics;

namespace InterStyle.Leads.Api;

public static class LeadsApi
{
    private static readonly ActivitySource ActivitySource = new("InterStyle.Leads.Api");

    public static RouteGroupBuilder MapLeadsApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/leads")
            .HasApiVersion(1.0)
            .WithTags("Leads");

        api.MapPost("", async (CreateLeadCommand command, IMediator mediator, ILoggerFactory loggerFactory, CancellationToken ct) =>
        {
            using var activity = ActivitySource.StartActivity("CreateLead");
            var logger = loggerFactory.CreateLogger("LeadsApi");

            logger.LogInformation("Creating lead");

            var id = await mediator.Send(command, ct);

            activity?.SetTag("lead.id", id.Value);
            logger.LogInformation("Lead {LeadId} created successfully", id.Value);

            return Results.Ok(new { id = id.Value });
        });

        api.MapGet("statistics", async (
            DateTimeOffset? fromUtc,
            DateTimeOffset? toUtc,
            ILeadQueries queries,
            ILoggerFactory loggerFactory,
            CancellationToken ct) =>
        {
            using var activity = ActivitySource.StartActivity("GetLeadStatistics");
            var logger = loggerFactory.CreateLogger("LeadsApi");

            var now = DateTimeOffset.UtcNow;
            var from = fromUtc ?? now.AddDays(-30);
            var to = toUtc ?? now;

            activity?.SetTag("statistics.from", from.ToString("o"));
            activity?.SetTag("statistics.to", to.ToString("o"));

            if (from >= to)
            {
                activity?.SetStatus(ActivityStatusCode.Error, "Invalid date range");
                logger.LogWarning("Invalid date range for lead statistics: {FromUtc} >= {ToUtc}", from, to);
                return Results.BadRequest(new { error = "fromUtc must be earlier than toUtc." });
            }

            logger.LogInformation("Retrieving lead statistics from {FromUtc} to {ToUtc}", from, to);

            var stats = await queries.GetStatisticsAsync(from, to, ct);
            return Results.Ok(stats);
        }).RequireAuthorization(InterStylePolicies.AdminOnly);

        return api;
    }
}
