using Refit;

namespace AdminPanel.Services;

public interface ICurtainsApi
{
    [Get("/api/curtains")]
    Task<IReadOnlyList<CurtainDto>> GetAllAsync();

    [Multipart]
    [Post("/api/curtains")]
    Task<ApiResponse<CreateCurtainResponse>> CreateAsync(
        [AliasAs("Picture")] StreamPart picture,
        [AliasAs("Preview")] StreamPart preview,
        [AliasAs("Name")] string name,
        [AliasAs("Description")] string description);

    [Put("/api/curtains/{id}/name")]
    Task UpdateNameAsync(Guid id, [Body] ChangeCurtainNameRequest request);

    [Put("/api/curtains/{id}/description")]
    Task UpdateDescriptionAsync(Guid id, [Body] ChangeCurtainDescriptionRequest request);

    [Multipart]
    [Put("/api/curtains/{id}/picture")]
    Task UpdatePictureAsync(Guid id, [AliasAs("Picture")] StreamPart picture);

    [Multipart]
    [Put("/api/curtains/{id}/preview")]
    Task UpdatePreviewAsync(Guid id, [AliasAs("Picture")] StreamPart picture);
}

public sealed record CurtainDto(
    Guid Id,
    string Name,
    string Description,
    string PictureUrl,
    string PreviewUrl);

public sealed record ChangeCurtainNameRequest(Guid CurtainId, string NewName);

public sealed record ChangeCurtainDescriptionRequest(Guid CurtainId, string NewDescription);

public sealed record CreateCurtainResponse(Guid Id);
