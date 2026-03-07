using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Claims;

namespace AdminPanel;

public sealed class JwtAuthenticationStateProvider(IJSRuntime jsRuntime) : AuthenticationStateProvider
{
    private const string TokenStorageKey = "jwt_token";

    private readonly IJSRuntime _jsRuntime = jsRuntime;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        string? token;

        try
        {
            token = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", TokenStorageKey);
        }
        catch
        {
            return JwtAuthenticationState.NotAuthenticated;
        }

        if (string.IsNullOrWhiteSpace(token))
        {
            return JwtAuthenticationState.NotAuthenticated;
        }

        JwtSecurityToken jwtToken;

        try
        {
            jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
        }
        catch
        {
            await RemoveTokenAsync();
            return JwtAuthenticationState.NotAuthenticated;
        }

        if (IsExpired(jwtToken))
        {
            await RemoveTokenAsync();
            return JwtAuthenticationState.NotAuthenticated;
        }

        return new JwtAuthenticationState(jwtToken);
    }

    public async Task SetTokenAsync(string token)
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", TokenStorageKey, token);

        var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
        var state = new JwtAuthenticationState(jwtToken);

        NotifyAuthenticationStateChanged(Task.FromResult<AuthenticationState>(state));
    }

    public async Task ClearTokenAsync()
    {
        await RemoveTokenAsync();
        NotifyAuthenticationStateChanged(Task.FromResult(JwtAuthenticationState.NotAuthenticated));
    }

    private async Task RemoveTokenAsync()
    {
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

    private sealed class JwtAuthenticationState(JwtSecurityToken token) : AuthenticationState(CreateClaimsPrincipal(token))
    {
        public static readonly AuthenticationState NotAuthenticated =
            new(new ClaimsPrincipal(new ClaimsIdentity()));

        public JwtSecurityToken Token { get; } = token;

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