namespace InterStyle.ImageApi.Services;

public interface IImageStorage
{
    public Task<string> SaveOriginalAsync(
        Guid imageId,
        string fileName,
        Stream content,
        CancellationToken cancellationToken = default);

    public Task SaveOptimizedAsync(
       Guid imageId,
       Stream content,
       CancellationToken cancellationToken = default);

    public ValueTask<string> GetOptimizedPath(Guid imageId, CancellationToken cancellationToken = default);

    public ValueTask<bool> OptimizedExists(Guid imageId, CancellationToken cancellationToken = default);
}
