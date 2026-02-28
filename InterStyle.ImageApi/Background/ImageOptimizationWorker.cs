using InterStyle.ImageApi.Application.Commands.OptimizeImage;
using InterStyle.ImageApi.Services;

namespace InterStyle.ImageApi.Background;

public sealed class ImageOptimizationWorker(
    IServiceScopeFactory scopeFactory,
    IImageJobQueue queue) : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly IImageJobQueue _queue = queue;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var command = await _queue.DequeueAsync(stoppingToken);

            using var scope = _scopeFactory.CreateScope();
            var handler = scope.ServiceProvider
                .GetRequiredService<OptimizeImageCommandHandler>();

            await handler.Handle(command, stoppingToken);
        }
    }
}