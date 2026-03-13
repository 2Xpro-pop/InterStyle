using Refit;

namespace AdminPanel.Services;

public interface ICurtainsApi
{
    [Get("/api/curtains")]
    Task<IReadOnlyList<CurtainDto>> GetAllAsync([Query] string locale);

    [Get("/api/curtains/{id}")]
    Task<CurtainDto?> GetByIdAsync(Guid id, [Query] string locale);

    [Multipart]
    [Post("/api/curtains")]
    Task<ApiResponse<CreateCurtainResponse>> CreateAsync(
        [AliasAs("Picture")] StreamPart picture,
        [AliasAs("Preview")] StreamPart preview,
        [AliasAs("Name")] string name,
        [AliasAs("Description")] string description);

    [Put("/api/curtains/{id}/translations")]
    Task UpsertTranslationAsync(Guid id, [Body] UpsertCurtainTranslationRequest request);

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
    string Locale,
    string PictureUrl,
    string PreviewUrl);

public sealed record UpsertCurtainTranslationRequest(
    Guid CurtainId,
    string Locale,
    string Name,
    string Description);

public sealed record CreateCurtainResponse(Guid Id);
