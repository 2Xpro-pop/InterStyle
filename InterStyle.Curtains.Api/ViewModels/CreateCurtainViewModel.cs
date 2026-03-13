using Microsoft.AspNetCore.Http;

namespace InterStyle.Curtains.Api.ViewModels;

public sealed record CreateCurtainViewModel(
    IFormFile Picture,
    IFormFile Preview,
    string Locale,
    string Name,
    string Description);
