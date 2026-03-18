namespace InterStyle.ImageApi.Services;

public sealed class ImageStorage : IImageStorage
{
    private readonly string _originalsPath;
    private readonly string _optimizedPath;

    public ImageStorage(IWebHostEnvironment environment)
    {
        _originalsPath = Path.Combine(environment.WebRootPath, "assets", "originals");
        _optimizedPath = Path.Combine(environment.WebRootPath, "assets", "optimized");

        Directory.CreateDirectory(_originalsPath);
        Directory.CreateDirectory(_optimizedPath);
    }

    public async Task<string> SaveOriginalAsync(
        Guid imageId,
        string fileName,
        Stream content,
        CancellationToken cancellationToken)
    {
        var ext = Path.GetExtension(fileName);
        var path = Path.Combine(_originalsPath, imageId.ToString("N") + ext);

        await using var fileStream = File.Create(path);
        await content.CopyToAsync(fileStream, cancellationToken);

        return path;
    }

    public async Task SaveOptimizedAsync(
        Guid imageId,
        Stream content,
        CancellationToken cancellationToken)
    {
        var path = GetOptimizedPath(imageId);

        await using var fileStream = File.Create(path);
        content.Position = 0;
        await content.CopyToAsync(fileStream, cancellationToken);
    }

    ValueTask<string> IImageStorage.GetOptimizedPath(Guid imageId, CancellationToken cancellationToken) 
        => ValueTask.FromResult(GetOptimizedPath(imageId));

    public string GetOptimizedPath(Guid imageId)
        => Path.Combine(_optimizedPath, imageId.ToString("N") + ".jpg");

    ValueTask<bool> IImageStorage.OptimizedExists(Guid imageId, CancellationToken cancellationToken)
        => ValueTask.FromResult(OptimizedExists(imageId));
    public bool OptimizedExists(Guid imageId)
        => File.Exists(GetOptimizedPath(imageId));
}