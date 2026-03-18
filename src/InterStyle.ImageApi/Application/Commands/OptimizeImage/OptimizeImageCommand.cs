namespace InterStyle.ImageApi.Application.Commands.OptimizeImage;

public sealed record OptimizeImageCommand(
    Guid ImageId,
    string OriginalPath,
    string OptimizedPath,
    string ContentType
);