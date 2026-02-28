using InterStyle.Curtains.Application.Commands;
using InterStyle.Curtains.Application.Queries;
using MediatR;

namespace InterStyle.Curtains.Api;

public static class CurtainsApi
{
    public static RouteGroupBuilder MapCurainsApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/curtains")
            .HasApiVersion(1.0)
            .WithTags("Curtains");

        api.MapPost("", async (CreateCurtainCommand command, IMediator mediator, CancellationToken ct) =>
        {
            var id = await mediator.Send(command, ct);
            return Results.Created($"/api/curtains/{id.Value}", new { id = id.Value });
        });

        api.MapGet("", async (ICurtainQueries queries, CancellationToken ct) =>
        {
            var result = await queries.GetAllAsync(ct);
            return Results.Ok(result);
        });

        api.MapGet("{id:guid}", async (Guid id, ICurtainQueries queries, CancellationToken ct) =>
        {
            var result = await queries.GetByIdAsync(id, ct);
            return result is not null ? Results.Ok(result) : Results.NotFound();
        });

        api.MapPut("{id:guid}/name", async (Guid id, ChangeCurtainNameCommand command, IMediator mediator, CancellationToken ct) =>
        {
            if (id != command.CurtainId)
            {
                return Results.BadRequest(new { error = "Route id does not match command id." });
            }

            await mediator.Send(command, ct);
            return Results.NoContent();
        });

        api.MapPut("{id:guid}/description", async (Guid id, ChangeCurtainDescriptionCommand command, IMediator mediator, CancellationToken ct) =>
        {
            if (id != command.CurtainId)
            {
                return Results.BadRequest(new { error = "Route id does not match command id." });
            }

            await mediator.Send(command, ct);
            return Results.NoContent();
        });

        api.MapPut("{id:guid}/picture", async (Guid id, ChangeCurtainPictureCommand command, IMediator mediator, CancellationToken ct) =>
        {
            if (id != command.CurtainId)
            {
                return Results.BadRequest(new { error = "Route id does not match command id." });
            }

            await mediator.Send(command, ct);
            return Results.NoContent();
        });

        api.MapPut("{id:guid}/preview", async (Guid id, ChangeCurtainPreviewCommand command, IMediator mediator, CancellationToken ct) =>
        {
            if (id != command.CurtainId)
            {
                return Results.BadRequest(new { error = "Route id does not match command id." });
            }

            await mediator.Send(command, ct);
            return Results.NoContent();
        });

        return api;
    }
}
