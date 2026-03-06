using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace InterStyle.ApiShared.Auth;

public static class JwtAuthExtensions
{
    public static IServiceCollection AddInterStyleJwtAuth(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtSection = configuration.GetSection("Jwt");
        var jwt = jwtSection.Get<JwtOptions>()
                  ?? throw new InvalidOperationException("Jwt section is not configured");

        services.AddOptions<JwtOptions>()
            .Bind(jwtSection)
            .ValidateOnStart();

        if (string.IsNullOrWhiteSpace(jwt.Authority))
            throw new InvalidOperationException("Jwt:Authority must be configured");

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = jwt.Authority;
                options.RequireHttpsMetadata = false; // dev
                options.RefreshOnIssuerKeyNotFound = true; // key rotation friendly

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwt.Issuer,

                    ValidateAudience = true,
                    ValidAudience = jwt.Audience,

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30),
                };
            });

        services.AddAuthorizationBuilder()
            .AddPolicy(InterStylePolicies.AdminOnly,
                p => p.RequireRole(InterStyleRoles.Admin));

        return services;
    }
}