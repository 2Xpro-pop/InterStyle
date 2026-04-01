using System.Diagnostics;
using InterStyle.ApiShared;
using InterStyle.ApiShared.Events;
using InterStyle.ImageApi.Background;
using InterStyle.ImageApi.Services;

namespace InterStyle.ImageApi.Application.Commands.OptimizeImage;

public sealed class OptimizeImageCommandHandler(
    IImageOptimizer optimizer,
    IImageStorage storage,
    IMessagePublisher publisher,
    ILogger<OptimizeImageCommandHandler> logger)
{
    private static readonly ActivitySource ActivitySource = new("InterStyle.ImageApi");
    private readonly IImageOptimizer _optimizer = optimizer;
    private readonly IImageStorage _storage = storage;
    private readonly IMessagePublisher _publisher = publisher;
    private readonly ILogger _logger = logger;

    public async Task Handle(
        OptimizeImageCommand command,
        CancellationToken cancellationToken)
    {
        using var activity = ActivitySource.StartActivity("OptimizeImage");
        activity?.AddTag("image.id", command.ImageId);
        activity?.AddTag("image.content_type", command.ContentType);

        _logger.LogInformation("Starting optimization for image {ImageId}, content type {ContentType}",
            command.ImageId, command.ContentType);

        await using var originalStream = File.OpenRead(command.OriginalPath);

        await using var optimizedStream =
            await _optimizer.Optimize(originalStream, command.ContentType, cancellationToken);

        _logger.LogInformation("Image {ImageId} optimized, saving result", command.ImageId);

        await _storage.SaveOptimizedAsync(
            command.ImageId,
            optimizedStream,
            cancellationToken);

        _logger.LogInformation("Optimized image {ImageId} saved, publishing event", command.ImageId);

        await _publisher.PublishAsync(
            new ImageOptimizedEvent(command.ImageId, ImageOptimizingStatus.Ready),
            cancellationToken);

        _logger.LogInformation("Image {ImageId} optimization completed with status {Status}",
            command.ImageId, ImageOptimizingStatus.Ready);
    }
}