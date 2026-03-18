using InterStyle.ApiShared.Events;
using InterStyle.ImageApi.Background;
using InterStyle.ImageApi.Services;

namespace InterStyle.ImageApi.Application.Commands.OptimizeImage;

public sealed class OptimizeImageCommandHandler
{
    private readonly IImageOptimizer _optimizer;
    private readonly IImageStorage _storage;
    private readonly IMessagePublisher _publisher;

    public OptimizeImageCommandHandler(
        IImageOptimizer optimizer,
        IImageStorage storage,
        IMessagePublisher publisher)
    {
        _optimizer = optimizer;
        _storage = storage;
        _publisher = publisher;
    }

    public async Task Handle(
        OptimizeImageCommand command,
        CancellationToken cancellationToken)
    {
        await using var originalStream = File.OpenRead(command.OriginalPath);

        await using var optimizedStream =
            await _optimizer.Optimize(originalStream, command.ContentType, cancellationToken);

        await _storage.SaveOptimizedAsync(
            command.ImageId,
            optimizedStream,
            cancellationToken);

        await _publisher.PublishAsync(
            new ImageOptimizedEvent(command.ImageId, ImageOptimizingStatus.Ready),
            cancellationToken);
    }
}