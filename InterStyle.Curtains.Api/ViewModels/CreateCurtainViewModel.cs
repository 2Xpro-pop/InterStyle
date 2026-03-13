using Microsoft.AspNetCore.Http;

namespace InterStyle.Curtains.Api.ViewModels;

public sealed record CreateCurtainViewModel(
    IFormFile Picture,
    IFormFile Preview,
    string Name,
    string Description);
