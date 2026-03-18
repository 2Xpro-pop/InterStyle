using InterStyle.ImageApi.Application.Commands.OptimizeImage;
using System.Threading.Channels;

namespace InterStyle.ImageApi.Services;

public sealed class ImageJobQueue : IImageJobQueue
{
    private readonly Channel<OptimizeImageCommand> channel =
        Channel.CreateUnbounded<OptimizeImageCommand>();

    public async ValueTask EnqueueAsync(object job, CancellationToken cancellationToken)
    {
        await channel.Writer.WriteAsync((OptimizeImageCommand)job, cancellationToken);
    }

    public async ValueTask<OptimizeImageCommand> DequeueAsync(CancellationToken cancellationToken)
    {
        return await channel.Reader.ReadAsync(cancellationToken);
    }
}