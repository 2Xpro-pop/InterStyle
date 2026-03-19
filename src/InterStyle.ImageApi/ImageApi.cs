using System.Diagnostics;
using InterStyle.ApiShared;
using InterStyle.ImageApi.Application.Commands.OptimizeImage;
using InterStyle.ImageApi.Application.Commands.UploadImage;
using InterStyle.ImageApi.Application.Queries.GetImageStatus;
using InterStyle.ImageApi.Application.Queries.GetOptimizedImage;
using Microsoft.Extensions.DependencyInjection;

namespace InterStyle.ImageApi;

public static class ImageApi
{
    private static readonly ActivitySource ActivitySource = new("InterStyle.ImageApi");
    public static RouteGroupBuilder MapImageApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/images")
            .HasApiVersion(1.0)
            .WithTags("Images");

        api.MapGet("", () =>
        {
            using var activity = ActivitySource.StartActivity("ListImages");

            var environment = app.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
            var assets = environment.WebRootPath;

            var files = Directory.GetFiles(assets)
                .Where(file => file.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                               file.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                               file.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                               file.EndsWith(".gif", StringComparison.OrdinalIgnoreCase))
                .Select(Path.GetFileName)
                .ToArray();

            activity?.AddTag("image.count", files.Length);

            return files;
        });

        api.MapPost("", async (
                    IFormFile file,
                    UploadImageCommandHandler handler,
                    CancellationToken cancellationToken) =>
        {
            using var activity = ActivitySource.StartActivity("UploadImage");
            activity?.AddTag("image.filename", file.FileName);
            activity?.AddTag("image.content_type", file.ContentType);
            activity?.AddTag("image.size", file.Length);

            await using var stream = file.OpenReadStream();

            var result = await handler.Handle(
                new UploadImageCommand(stream, file.FileName, file.ContentType),
                cancellationToken);

            activity?.AddTag("image.id", result.ImageId);

            return Results.Accepted($"/images/{result.ImageId}", result);
        }).DisableAntiforgery();

        api.MapGet("{id:guid}/status", async (
            Guid id,
            GetImageStatusQueryHandler handler,
            CancellationToken cancellationToken) =>
        {
            using var activity = ActivitySource.StartActivity("GetImageStatus");
            activity?.AddTag("image.id", id);

            return Results.Ok(await handler.Handle(
                new GetImageStatusQuery(id),
                cancellationToken));
        });

        api.MapGet("{id:guid}", async (
            Guid id,
            GetOptimizedImageQueryHandler handler,
            CancellationToken cancellationToken) =>
        {
            using var activity = ActivitySource.StartActivity("GetOptimizedImage");
            activity?.AddTag("image.id", id);

            var result = await handler.Handle(
                new GetOptimizedImageQuery(id),
                cancellationToken);

            if (result is null)
            {
                activity?.SetStatus(ActivityStatusCode.Error, "Image not found");
                return Results.NotFound();
            }

            return Results.File(result, "image/jpeg");
        });

        return api;
    }
}
