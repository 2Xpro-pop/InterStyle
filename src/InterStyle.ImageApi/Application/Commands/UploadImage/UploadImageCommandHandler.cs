using System.Diagnostics;
using InterStyle.ImageApi.Application.Commands.OptimizeImage;
using InterStyle.ImageApi.Services;

namespace InterStyle.ImageApi.Application.Commands.UploadImage;

public sealed class UploadImageCommandHandler(
    IImageStorage storage,
    IImageJobQueue queue,
    ILogger<UploadImageCommandHandler> logger)
{
    private static readonly ActivitySource ActivitySource = new("InterStyle.ImageApi");
    private readonly IImageStorage _storage = storage;
    private readonly IImageJobQueue _queue = queue;
    private readonly ILogger _logger = logger;

    public async Task<UploadImageResult> Handle(
        UploadImageCommand command,
        CancellationToken cancellationToken)
    {
        var imageId = Guid.NewGuid();

        using var activity = ActivitySource.StartActivity("SaveOriginalImage");
        activity?.AddTag("image.id", imageId);
        activity?.AddTag("image.filename", command.FileName);
        activity?.AddTag("image.content_type", command.ContentType);

        _logger.LogInformation("Saving original image {ImageId} with filename {FileName}, content type {ContentType}",
            imageId, command.FileName, command.ContentType);

        var originalPath = await _storage.SaveOriginalAsync(
            imageId,
            command.FileName,
            command.Content,
            cancellationToken);

        _logger.LogInformation("Original image {ImageId} saved to {OriginalPath}", imageId, originalPath);

        await _queue.EnqueueAsync(
            new OptimizeImageCommand(
                imageId,
                originalPath,
                await _storage.GetOptimizedPath(imageId, cancellationToken),
                command.ContentType),
            cancellationToken);

        _logger.LogInformation("Optimization job enqueued for image {ImageId}", imageId);

        return new UploadImageResult(imageId);
    }
}