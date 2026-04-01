using InterStyle.ApiShared.Events;
using InterStyle.ImageApi.Services;

namespace InterStyle.ImageApi.Application.Queries.GetImageStatus;

public sealed class GetImageStatusQueryHandler(IImageStorage storage, ILogger<GetImageStatusQueryHandler> logger)
{
    private readonly IImageStorage _storage = storage;
    private readonly ILogger _logger = logger;

    public async Task<object> Handle(
        GetImageStatusQuery query,
        CancellationToken cancellationToken)
    {
        var exists = await _storage.OptimizedExists(query.ImageId, cancellationToken);
        var status = exists ? ImageOptimizingStatus.Ready : ImageOptimizingStatus.Processing;

        _logger.LogInformation("Image status checked for {ImageId}: {Status}", query.ImageId, status);

        return Task.FromResult<object>(new ImageOptimizedEvent(query.ImageId, status));
    }
}