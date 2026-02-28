using InterStyle.ImageApi.Application.Commands.OptimizeImage;
using InterStyle.ImageApi.Application.Commands.UploadImage;
using InterStyle.ImageApi.Application.Queries.GetImageStatus;
using InterStyle.ImageApi.Application.Queries.GetOptimizedImage;
using Microsoft.Extensions.DependencyInjection;

namespace InterStyle.ImageApi;

public static class ImageApi
{
    public static RouteGroupBuilder MapImageApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("images")
            .HasApiVersion(1.0)
            .WithTags("Images");

        api.MapGet("", () =>
        {
            var environment = app.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
            var assets = environment.WebRootPath;

            var files = Directory.GetFiles(assets)
                .Where(file => file.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                               file.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                               file.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                               file.EndsWith(".gif", StringComparison.OrdinalIgnoreCase))
                .Select(Path.GetFileName)
                .ToArray();

            return files;
        });

        api.MapPost("", async (
                    IFormFile file,
                    UploadImageCommandHandler handler,
                    CancellationToken cancellationToken) =>
        {
            await using var stream = file.OpenReadStream();

            var result = await handler.Handle(
                new UploadImageCommand(stream, file.FileName, file.ContentType),
                cancellationToken);

            return Results.Accepted($"/images/{result.ImageId}", result);
        }).DisableAntiforgery();

        api.MapGet("{id:guid}/status", async (
            Guid id,
            GetImageStatusQueryHandler handler,
            CancellationToken cancellationToken) =>
        {
            return Results.Ok(await handler.Handle(
                new GetImageStatusQuery(id),
                cancellationToken));
        });

        api.MapGet("{id:guid}", async (
            Guid id,
            GetOptimizedImageQueryHandler handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(
                new GetOptimizedImageQuery(id),
                cancellationToken);

            return result is null
                ? Results.NotFound()
                : Results.File(result, "image/jpeg");
        });

        return api;
    }
}
