using System.Diagnostics;
using InterStyle.ApiShared;
using InterStyle.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;

namespace InterStyle.IdentityApi;

public static class IdentityApi
{
    private static readonly ActivitySource ActivitySource = new("InterStyle.IdentityApi");

    public static RouteGroupBuilder MapIdentityApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/identity")
            .HasApiVersion(1.0)
            .WithTags("Identity");

        api.MapPost("/login", async (
            Shared.LoginRequest request,
            UserManager<AdminUser> userManager,
            JwtTokenIssuer issuer,
            ILoggerFactory loggerFactory) =>
        {
            using var activity = ActivitySource.StartActivity("Login");
            var logger = loggerFactory.CreateLogger("IdentityApi");

            activity?.AddTag("identity.username", request.Username);
            logger.LogInformation("Login attempt for user {Username}", request.Username);

            var user = await userManager.FindByNameAsync(request.Username);
            if (user is null)
            {
                activity?.SetStatus(ActivityStatusCode.Error, "User not found");
                logger.LogWarning("Login failed: user {Username} not found", request.Username);
                return Results.Unauthorized();
            }

            var valid = await userManager.CheckPasswordAsync(user, request.Password);
            if (!valid)
            {
                activity?.SetStatus(ActivityStatusCode.Error, "Invalid password");
                logger.LogWarning("Login failed: invalid password for user {Username}", request.Username);
                return Results.Unauthorized();
            }

            var token = issuer.IssueAdminToken(user.Id, user.UserName!);

            logger.LogInformation("Login succeeded for user {Username}", request.Username);
            return Results.Ok(new TokenResponse(token));
        })
        .AllowAnonymous();

        return api;
    }
}
