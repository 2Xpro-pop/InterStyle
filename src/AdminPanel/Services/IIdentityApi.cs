using InterStyle.Shared;
using Refit;

namespace AdminPanel.Services;

public interface IIdentityApi
{
    [Post("/api/identity/login")]
    public Task<TokenResponse> Login(LoginRequest loginRequest);
}
