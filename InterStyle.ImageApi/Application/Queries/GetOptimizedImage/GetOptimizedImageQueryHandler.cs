using InterStyle.ImageApi.Services;

namespace InterStyle.ImageApi.Application.Queries.GetOptimizedImage;

public sealed class GetOptimizedImageQueryHandler(IImageStorage storage)
{
    private readonly IImageStorage _storage = storage;

    public async Task<Stream?> Handle(
        GetOptimizedImageQuery query,
        CancellationToken cancellationToken)
    {
        var path = await _storage.GetOptimizedPath(query.ImageId, cancellationToken);

        return !File.Exists(path) ? null : (Stream)File.OpenRead(path);
    }
}