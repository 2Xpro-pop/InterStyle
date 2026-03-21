using System.Diagnostics;
using InterStyle.ApiShared;
using InterStyle.ImageApi.Application.Commands.OptimizeImage;
using InterStyle.ImageApi.Services;

namespace InterStyle.ImageApi.Background;

public sealed class ImageOptimizationWorker(
    IServiceScopeFactory scopeFactory,
    IImageJobQueue queue,
    ILoggerFactory loggerFactory) : BackgroundService
{
    private static readonly ActivitySource ActivitySource = new("InterStyle.ImageApi");
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly IImageJobQueue _queue = queue;
    private readonly ILogger _logger = loggerFactory.CreateLogger<ImageOptimizationWorker>();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Image optimization worker started");

        while (!stoppingToken.IsCancellationRequested)
        {
            var command = await _queue.DequeueAsync(stoppingToken);

            using var activity = ActivitySource.StartActivity("ProcessOptimizationJob");
            activity?.AddTag("image.id", command.ImageId);
            activity?.AddTag("image.content_type", command.ContentType);

            _logger.LogInformation("Dequeued optimization job for image {ImageId}, content type {ContentType}",
                command.ImageId, command.ContentType);

            try
            {
                using var scope = _scopeFactory.CreateScope();
                var handler = scope.ServiceProvider
                    .GetRequiredService<OptimizeImageCommandHandler>();

                await handler.Handle(command, stoppingToken);

                _logger.LogInformation("Optimization job completed for image {ImageId}", command.ImageId);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                activity?.SetExceptionTags(ex);
                _logger.LogError(ex, "Optimization job failed for image {ImageId}", command.ImageId);
            }
        }

        _logger.LogInformation("Image optimization worker stopping");
    }
}