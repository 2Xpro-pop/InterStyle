using InterStyle.Shared;

namespace AdminPanel.Services;

public sealed class AuthService(IIdentityApi identityApi, IJwtTokenSetter jwtTokenSetter): IAuthService
{
    private readonly IIdentityApi _identityApi = identityApi;
    private readonly IJwtTokenSetter _jwtTokenSetter = jwtTokenSetter;

    public async Task Login(string login, string password, bool rememberMe)
    {
        var request = new LoginRequest(login, password);

         var token = await _identityApi.Login(request);
         await _jwtTokenSetter.SetTokenAsync(token.Token, rememberMe);
    }
}
