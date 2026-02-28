using InterStyle.ApiShared.Events;
using InterStyle.ImageApi.Services;

namespace InterStyle.ImageApi.Application.Queries.GetImageStatus;

public sealed class GetImageStatusQueryHandler(IImageStorage storage)
{
    private readonly IImageStorage _storage = storage;

    public async Task<object> Handle(
        GetImageStatusQuery query,
        CancellationToken cancellationToken)
    {
        var exists = await _storage.OptimizedExists(query.ImageId, cancellationToken);

        return Task.FromResult<object>(new ImageOptimizedEvent
        (
            query.ImageId,
            exists ? ImageOptimizingStatus.Ready: ImageOptimizingStatus.Processing
        ));
    }
}