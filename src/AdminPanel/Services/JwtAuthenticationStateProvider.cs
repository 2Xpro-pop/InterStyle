using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Claims;

namespace AdminPanel.Services;

public sealed class JwtAuthenticationStateProvider(IJSRuntime jsRuntime, ILogger<JwtAuthenticationStateProvider> logger) : AuthenticationStateProvider, IJwtTokenSetter
{
    private const string TokenStorageKey = "jwt_token";

    private readonly IJSRuntime _jsRuntime = jsRuntime;
    private readonly ILogger<JwtAuthenticationStateProvider> _logger = logger;
    private JwtAuthenticationState? _state = null;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if(_state != null && !_state.IsExpired)
        {
            return _state;
        }

        string? token;

        try
        {
            token = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", TokenStorageKey);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to read JWT token from localStorage");
            return JwtAuthenticationState.NotAuthenticated;
        }

        if (string.IsNullOrWhiteSpace(token))
        {
            _logger.LogDebug("No JWT token found in localStorage");
            return JwtAuthenticationState.NotAuthenticated;
        }

        JwtSecurityToken jwtToken;

        try
        {
            jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to parse JWT token, removing from storage");
            await RemoveTokenAsync();
            return JwtAuthenticationState.NotAuthenticated;
        }

        if (IsExpired(jwtToken))
        {
            _logger.LogInformation("JWT token expired at {ExpiresAtUtc}, removing from storage", jwtToken.ValidTo);
            await RemoveTokenAsync();
            return JwtAuthenticationState.NotAuthenticated;
        }

        _logger.LogDebug("JWT token valid, expires at {ExpiresAtUtc}", jwtToken.ValidTo);
        return _state = new JwtAuthenticationState(jwtToken);
    }

    public async Task SetTokenAsync(string token, bool remember)
    {
        if(remember)
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", TokenStorageKey, token);
            _logger.LogInformation("JWT token stored in localStorage, remember={RememberMe}", remember);
        }

        var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
        _state = new JwtAuthenticationState(jwtToken);

        _logger.LogInformation("Authentication state updated, token expires at {ExpiresAtUtc}", jwtToken.ValidTo);
        NotifyAuthenticationStateChanged(Task.FromResult<AuthenticationState>(_state));
    }

    public async Task ClearTokenAsync()
    {
        _logger.LogInformation("Clearing authentication state and removing JWT token");
        await RemoveTokenAsync();
        NotifyAuthenticationStateChanged(Task.FromResult(JwtAuthenticationState.NotAuthenticated));
    }

    private async Task RemoveTokenAsync()
    {
        _state = null;
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", TokenStorageKey);
    }

    private static bool IsExpired(JwtSecurityToken token)
    {
        var expiresAtUtc = token.ValidTo;

        if (expiresAtUtc == DateTime.MinValue)
        {
            return true;
        }

        return expiresAtUtc <= DateTime.UtcNow;
    }

    internal sealed class JwtAuthenticationState(JwtSecurityToken token) : AuthenticationState(CreateClaimsPrincipal(token))
    {
        public static readonly AuthenticationState NotAuthenticated =
            new(new ClaimsPrincipal(new ClaimsIdentity()));

        public JwtSecurityToken Token { get; } = token;

        public bool IsExpired => IsExpired(Token);

        private static ClaimsPrincipal CreateClaimsPrincipal(JwtSecurityToken token)
        {
            var claims = token.Claims.ToList();

            var roleClaims = claims
                .Where(x => x.Type == "role" || x.Type == "roles")
                .Select(x => new Claim(ClaimTypes.Role, x.Value))
                .ToList();

            claims.AddRange(roleClaims);

            return new ClaimsPrincipal(
                new ClaimsIdentity(
                    claims,
                    authenticationType: "Bearer",
                    nameType: ClaimTypes.Name,
                    roleType: ClaimTypes.Role));
        }
    }
}