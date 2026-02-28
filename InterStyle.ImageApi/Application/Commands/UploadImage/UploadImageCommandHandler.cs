using InterStyle.ImageApi.Application.Commands.OptimizeImage;
using InterStyle.ImageApi.Services;

namespace InterStyle.ImageApi.Application.Commands.UploadImage;

public sealed class UploadImageCommandHandler(
    IImageStorage storage,
    IImageJobQueue queue)
{
    private readonly IImageStorage _storage = storage;
    private readonly IImageJobQueue _queue = queue;

    public async Task<UploadImageResult> Handle(
        UploadImageCommand command,
        CancellationToken cancellationToken)
    {
        var imageId = Guid.NewGuid();

        var originalPath = await _storage.SaveOriginalAsync(
            imageId,
            command.FileName,
            command.Content,
            cancellationToken);

        await _queue.EnqueueAsync(
            new OptimizeImageCommand(
                imageId,
                originalPath,
                await _storage.GetOptimizedPath(imageId, cancellationToken),
                command.ContentType),
            cancellationToken);

        return new UploadImageResult(imageId);
    }
}