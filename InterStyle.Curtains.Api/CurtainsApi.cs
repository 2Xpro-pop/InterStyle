using InterStyle.ApiShared.Auth;
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
            ILoggerFactory loggerFactory,
            CancellationToken ct) =>
        {

            var logger = loggerFactory.CreateLogger("CurtainsApi");

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
                logger.LogError("Image service is unavailable.");
                return Results.Problem(
                    title: "Image service is unavailable",
                    statusCode: StatusCodes.Status503ServiceUnavailable);
            }

            var pictureUrl = $"/api/images/{pictureId}";
            var previewUrl = $"/api/images/{previewId}";

            var id = await mediator.Send(
                new CreateCurtainCommand(model.Name, model.Description, pictureUrl, previewUrl),
                ct);

            return Results.Created($"/api/curtains/{id.Value}", new { id = id.Value });
        }).DisableAntiforgery()
          .RequireAuthorization(InterStylePolicies.AdminOnly);


        api.MapGet("", async ([FromQuery] string? locale, ICurtainQueries queries, CancellationToken ct) =>
        {
            var effectiveLocale = locale ?? Domain.Locale.Default.Value;
            var result = await queries.GetAllAsync(effectiveLocale, ct);
            return Results.Ok(result);
        }).AllowAnonymous();

        api.MapGet("{id:guid}", async (Guid id, [FromQuery] string? locale, ICurtainQueries queries, CancellationToken ct) =>
        {
            var effectiveLocale = locale ?? Domain.Locale.Default.Value;
            var result = await queries.GetByIdAsync(id, effectiveLocale, ct);
            return result is not null ? Results.Ok(result) : Results.NotFound();
        }).AllowAnonymous();

        api.MapPut("{id:guid}/translations", async (Guid id, UpsertCurtainTranslationCommand command, IMediator mediator, CancellationToken ct) =>
        {
            if (id != command.CurtainId)
            {
                return Results.BadRequest(new { error = "Route id does not match command id." });
            }

            await mediator.Send(command, ct);
            return Results.NoContent();
        }).RequireAuthorization(InterStylePolicies.AdminOnly);

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

            var pictureUrl = $"/api/images/{imageId}";

            await mediator.Send(new ChangeCurtainPictureCommand(id, pictureUrl), ct);

            return Results.NoContent();
        }).DisableAntiforgery()
          .RequireAuthorization(InterStylePolicies.AdminOnly);

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

            var previewUrl = $"/api/images/{imageId}";

            await mediator.Send(new ChangeCurtainPreviewCommand(id, previewUrl), ct);

            return Results.NoContent();
        }).DisableAntiforgery()
          .RequireAuthorization(InterStylePolicies.AdminOnly);

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
