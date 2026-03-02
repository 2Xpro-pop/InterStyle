using InterStyle.Curtains.Api.ViewModels;
using InterStyle.Curtains.Application.Commands;
using InterStyle.Curtains.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace InterStyle.Curtains.Api;

public static class CurtainsApi
{
    public static RouteGroupBuilder MapCurainsApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/curtains")
            .HasApiVersion(1.0)
            .WithTags("Curtains");

        api.MapPost("", async (
            [FromForm] CreateCurtainViewModel model,
            IHttpClientFactory httpClientFactory,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var client = httpClientFactory.CreateClient("ImageApi");

            Guid pictureId;
            Guid previewId;

            try
            {
                pictureId = await UploadImageAsync(client, model.Picture, ct);
                previewId = await UploadImageAsync(client, model.Preview, ct);
            }
            catch (Exception)
            {
                return Results.Problem(
                    title: "Image service is unavailable",
                    statusCode: StatusCodes.Status503ServiceUnavailable);
            }

            var pictureUrl = new Uri(client.BaseAddress!, $"images/{pictureId}").ToString();
            var previewUrl = new Uri(client.BaseAddress!, $"images/{previewId}").ToString();

            var id = await mediator.Send(
                new CreateCurtainCommand(model.Name, model.Description, pictureUrl, previewUrl),
                ct);

            return Results.Created($"/api/curtains/{id.Value}", new { id = id.Value });
        }).DisableAntiforgery();


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

        api.MapPut("{id:guid}/picture", async (
            Guid id,
            [FromForm] ChangePictureViewModel model,
            IHttpClientFactory httpClientFactory,
            IMediator mediator,
            CancellationToken ct) =>
        {
            if (model.Picture is null || model.Picture.Length == 0)
            {
                return Results.BadRequest(new { error = "Picture file is required." });
            }

            var client = httpClientFactory.CreateClient("ImageApi");

            Guid imageId;
            try
            {
                imageId = await UploadImageAsync(client, model.Picture, ct);
            }
            catch (Exception)
            {
                return Results.Problem(
                    title: "Image service is unavailable",
                    statusCode: StatusCodes.Status503ServiceUnavailable);
            }

            var pictureUrl = new Uri(client.BaseAddress!, $"images/{imageId}").ToString();

            await mediator.Send(new ChangeCurtainPictureCommand(id, pictureUrl), ct);

            return Results.NoContent();
        }).DisableAntiforgery();

        api.MapPut("{id:guid}/preview", async (
            Guid id,
            [FromForm] ChangePictureViewModel model,
            IHttpClientFactory httpClientFactory,
            IMediator mediator,
            CancellationToken ct) =>
        {
            if (model.Picture is null || model.Picture.Length == 0)
            {
                return Results.BadRequest(new { error = "Picture file is required." });
            }

            var client = httpClientFactory.CreateClient("ImageApi");

            Guid imageId;
            try
            {
                imageId = await UploadImageAsync(client, model.Picture, ct);
            }
            catch (Exception)
            {
                return Results.Problem(
                    title: "Image service is unavailable",
                    statusCode: StatusCodes.Status503ServiceUnavailable);
            }

            var previewUrl = new Uri(client.BaseAddress!, $"images/{imageId}").ToString();

            await mediator.Send(new ChangeCurtainPreviewCommand(id, previewUrl), ct);

            return Results.NoContent();
        }).DisableAntiforgery();

        return api;
    }

    private static async Task<Guid> UploadImageAsync(
        HttpClient client,
        IFormFile file,
        CancellationToken cancellationToken)
    {
        using var content = new MultipartFormDataContent();
        await using var stream = file.OpenReadStream();
        using var fileContent = new StreamContent(stream);

        fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
        content.Add(fileContent, "file", file.FileName);

        using var response = await client.PostAsync("api/images", content, cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<UploadImageResponse>(cancellationToken: cancellationToken);

        return result?.ImageId
            ?? throw new InvalidOperationException("Image API did not return an image id.");
    }

    private sealed record UploadImageResponse(Guid ImageId);
}
