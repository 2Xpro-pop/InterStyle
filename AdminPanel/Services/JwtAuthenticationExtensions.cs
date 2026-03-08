using Microsoft.AspNetCore.Components.Authorization;

namespace AdminPanel.Services;

public static class JwtAuthenticationExtensions
{
    public static void AddJwtAuthentication(this IServiceCollection services)
    {
        services.AddSingleton<JwtAuthenticationStateProvider>();

        services.AddSingleton<AuthenticationStateProvider>(sp => sp.GetRequiredService<JwtAuthenticationStateProvider>());
        services.AddSingleton<IJwtTokenSetter>(sp => sp.GetRequiredService<JwtAuthenticationStateProvider>());
    }
}
