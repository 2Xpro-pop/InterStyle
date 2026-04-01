using System.Diagnostics;
using InterStyle.Shared;

namespace AdminPanel.Services;

public sealed class AuthService(IIdentityApi identityApi, IJwtTokenSetter jwtTokenSetter, ILogger<AuthService> logger): IAuthService
{
    private readonly IIdentityApi _identityApi = identityApi;
    private readonly IJwtTokenSetter _jwtTokenSetter = jwtTokenSetter;
    private readonly ILogger<AuthService> _logger = logger;

    public async Task Login(string login, string password, bool rememberMe)
    {
        using var activity = Telemetry.ActivitySource.StartActivity("Login");
        activity?.AddTag("identity.username", login);

        _logger.LogInformation("Attempting login for user {Username}", login);

        var request = new LoginRequest(login, password);

        var token = await _identityApi.Login(request);
        await _jwtTokenSetter.SetTokenAsync(token.Token, rememberMe);

        _logger.LogInformation("Login succeeded for user {Username}, remember={RememberMe}", login, rememberMe);
    }
}
