using InterStyle.ApiShared;
using InterStyle.ApiShared.Events;
using InterStyle.ImageApi.Application.Commands.OptimizeImage;
using InterStyle.ImageApi.Application.Commands.UploadImage;
using InterStyle.ImageApi.Application.Queries.GetImageStatus;
using InterStyle.ImageApi.Application.Queries.GetOptimizedImage;
using InterStyle.ImageApi.Background;
using InterStyle.ImageApi.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing.Constraints;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

builder.Services.AddSingleton<IMessagePublisher>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("rabbitmq")
        ?? throw new InvalidOperationException("RabbitMQ connection string missing");

    return new RabbitMqMessagePublisher(connectionString);
});

builder.Services.Configure<RouteOptions>(
    options => options.SetParameterPolicy<RegexInlineRouteConstraint>("regex"));

builder.Services.AddHealthChecks();

var withApiVersioning = builder.Services.AddApiVersioning();

builder.AddDefaultOpenApi(withApiVersioning);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IImageJobQueue, ImageJobQueue>();
builder.Services.AddSingleton<IImageStorage,ImageStorage>();
builder.Services.AddSingleton<IImageOptimizer, ImageOptimizer>();

builder.Services.AddScoped<UploadImageCommandHandler>();
builder.Services.AddScoped<OptimizeImageCommandHandler>();
builder.Services.AddScoped<GetImageStatusQueryHandler>();
builder.Services.AddScoped<GetOptimizedImageQueryHandler>();

builder.Services.AddHostedService<ImageOptimizationWorker>();

var app = builder.Build();

app.UseStaticFiles("/assets");

app.UseSwaggerDefaults();
app.UseHealthChecksDefaults();

app.UseDefaultOpenApi();

app.MapGet("/images", () =>
{
    var assets = app.Environment.WebRootPath;

    var files = Directory.GetFiles(assets)
        .Where(file => file.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                       file.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                       file.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                       file.EndsWith(".gif", StringComparison.OrdinalIgnoreCase))
        .Select(Path.GetFileName)
        .ToArray();

    return files;
});

app.MapPost("/images", async (
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

app.MapGet("/images/{id:guid}/status", async (
    Guid id,
    GetImageStatusQueryHandler handler,
    CancellationToken cancellationToken) =>
{
    return Results.Ok(await handler.Handle(
        new GetImageStatusQuery(id),
        cancellationToken));
});

app.MapGet("/images/{id:guid}", async (
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

app.Run();

[JsonSerializable(typeof(string[]))]
[JsonSerializable(typeof(ImageOptimizedEvent))]
[JsonSerializable(typeof(UploadImageResult))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}
