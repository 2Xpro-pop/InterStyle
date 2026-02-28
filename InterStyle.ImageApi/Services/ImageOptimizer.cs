using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace InterStyle.ImageApi.Services;

public sealed class ImageOptimizer : IImageOptimizer
{
    // Max dimension for resizing (keeps aspect ratio).
    private const int MaxSide = 1920;

    // JPEG quality (0..100).
    private const int JpegQuality = 82;

    public async Task<Stream> Optimize(Stream imageStream, string contentType, CancellationToken cancellationToken = default)
    {
        // ImageSharp async decode
        using var image = await Image.LoadAsync(imageStream, cancellationToken);

        var width = image.Width;
        var height = image.Height;

        // Resize only if needed
        var maxCurrentSide = Math.Max(width, height);
        if (maxCurrentSide > MaxSide)
        {
            var scale = (double)MaxSide / maxCurrentSide;
            var targetWidth = (int)Math.Round(width * scale);
            var targetHeight = (int)Math.Round(height * scale);

            image.Mutate(processing => processing.Resize(targetWidth, targetHeight));
        }

        // Encode as JPEG for photos (simple default strategy)
        var output = new MemoryStream();
        var encoder = new JpegEncoder { Quality = JpegQuality };

        await image.SaveAsJpegAsync(output, encoder, cancellationToken);
        output.Position = 0;
        return output;
    }
}