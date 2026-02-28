using InterStyle.ImageApi.Application.Commands.OptimizeImage;

namespace InterStyle.ImageApi.Services;
public interface IImageJobQueue
{
    ValueTask EnqueueAsync(object job, CancellationToken cancellationToken);
    ValueTask<OptimizeImageCommand> DequeueAsync(CancellationToken cancellationToken);
}