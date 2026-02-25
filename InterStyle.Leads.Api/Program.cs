using InterStyle.Leads.Api;
using InterStyle.Leads.Application.Commands;
using InterStyle.Leads.Application.Queries;
using InterStyle.Leads.Domain;
using InterStyle.Leads.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<CreateLeadCommand>());

builder.Services.AddDbContext<LeadsDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("leadsdb"));
});

builder.Services.AddMigration<LeadsDbContext>();

builder.Services.AddScoped<ILeadRepository, LeadRepository>();

builder.Services.AddScoped<ILeadQueries, LeadQueries>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

if (app.Environment.IsDevelopment())
{
    // All health checks must pass for app to be considered ready to accept traffic after starting
    app.MapHealthChecks("/health");

    // Only health checks tagged with the "live" tag must pass for app to be considered alive
    app.MapHealthChecks("/alive", new HealthCheckOptions
    {
        Predicate = r => r.Tags.Contains("live")
    });
}

app.MapPost("/leads", async (CreateLeadCommand command, IMediator mediator, CancellationToken ct) =>
{
    var id = await mediator.Send(command, ct);
    return Results.Ok(new { id = id.Value });
});

app.MapGet("/leads/statistics", async (
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

app.Run();