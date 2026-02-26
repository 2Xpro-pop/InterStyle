using InterStyle.ApiShared;
using InterStyle.Curtains.Application.Commands;
using InterStyle.Curtains.Application.Queries;
using InterStyle.Curtains.Domain;
using InterStyle.Curtains.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<CreateCurtainCommand>());

builder.Services.AddDefaultDbContext<CurtainsDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("curtainsdb"));
});

var withApiVersioning = builder.Services.AddApiVersioning();

builder.AddDefaultOpenApi(withApiVersioning);

builder.Services.AddScoped<ICurtainRepository, CurtainRepository>();

builder.Services.AddScoped<ICurtainQueries, CurtainsQueries>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwaggerDefaults();
app.UseHealthChecksDefaults();

app.UseDefaultOpenApi();

app.MapPost("/curtains", async (CreateCurtainCommand command, IMediator mediator, CancellationToken ct) =>
{
    var id = await mediator.Send(command, ct);
    return Results.Created($"/curtains/{id.Value}", new { id = id.Value });
});

app.MapGet("/curtains", async (ICurtainQueries queries, CancellationToken ct) =>
{
    var result = await queries.GetAllAsync(ct);
    return Results.Ok(result);
});

app.MapGet("/curtains/{id:guid}", async (Guid id, ICurtainQueries queries, CancellationToken ct) =>
{
    var result = await queries.GetByIdAsync(id, ct);
    return result is not null ? Results.Ok(result) : Results.NotFound();
});

app.MapPut("/curtains/{id:guid}/name", async (Guid id, ChangeCurtainNameCommand command, IMediator mediator, CancellationToken ct) =>
{
    if (id != command.CurtainId)
    {
        return Results.BadRequest(new { error = "Route id does not match command id." });
    }
    await mediator.Send(command, ct);
    return Results.NoContent();
});

app.MapPut("/curtains/{id:guid}/description", async (Guid id, ChangeCurtainDescriptionCommand command, IMediator mediator, CancellationToken ct) =>
{
    if (id != command.CurtainId)
    {
        return Results.BadRequest(new { error = "Route id does not match command id." });
    }
    await mediator.Send(command, ct);
    return Results.NoContent();
});

app.MapPut("/curtains/{id:guid}/picture", async (Guid id, ChangeCurtainPictureCommand command, IMediator mediator, CancellationToken ct) =>
{
    if (id != command.CurtainId)
    {
        return Results.BadRequest(new { error = "Route id does not match command id." });
    }
    await mediator.Send(command, ct);
    return Results.NoContent();
});

app.MapPut("/curtains/{id:guid}/preview", async (Guid id, ChangeCurtainPreviewCommand command, IMediator mediator, CancellationToken ct) =>
{
    if (id != command.CurtainId)
    {
        return Results.BadRequest(new { error = "Route id does not match command id." });
    }
    await mediator.Send(command, ct);
    return Results.NoContent();
});

app.Run();