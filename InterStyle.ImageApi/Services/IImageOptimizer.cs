namespace InterStyle.ImageApi.Services;

public interface IImageOptimizer
{
    public Task<Stream> Optimize(Stream imageStream, string contentType, CancellationToken cancellationToken = default);
}
