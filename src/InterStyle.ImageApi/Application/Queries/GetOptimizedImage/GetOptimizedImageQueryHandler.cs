using InterStyle.ImageApi.Services;

namespace InterStyle.ImageApi.Application.Queries.GetOptimizedImage;

public sealed class GetOptimizedImageQueryHandler(IImageStorage storage, ILogger<GetOptimizedImageQueryHandler> logger)
{
    private readonly IImageStorage _storage = storage;
    private readonly ILogger _logger = logger;

    public async Task<Stream?> Handle(
        GetOptimizedImageQuery query,
        CancellationToken cancellationToken)
    {
        var path = await _storage.GetOptimizedPath(query.ImageId, cancellationToken);

        if (!File.Exists(path))
        {
            _logger.LogWarning("Optimized image not found for {ImageId} at {Path}", query.ImageId, path);
            return null;
        }

        _logger.LogInformation("Serving optimized image {ImageId}", query.ImageId);
        return File.OpenRead(path);
    }
}