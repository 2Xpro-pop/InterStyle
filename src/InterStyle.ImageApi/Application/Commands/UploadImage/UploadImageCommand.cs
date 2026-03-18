namespace InterStyle.ImageApi.Application.Commands.UploadImage;

public sealed record UploadImageCommand(
    Stream Content,
    string FileName,
    string ContentType
);

public sealed record UploadImageResult(Guid ImageId);