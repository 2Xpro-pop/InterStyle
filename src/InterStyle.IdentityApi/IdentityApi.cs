using InterStyle.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;

namespace InterStyle.IdentityApi;

public static class IdentityApi
{
    public static RouteGroupBuilder MapIdentityApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/identity")
            .HasApiVersion(1.0)
            .WithTags("Identity");

        api.MapPost("/login", async (
            Shared.LoginRequest request,
            UserManager<AdminUser> userManager,
            JwtTokenIssuer issuer) =>
        {
            var user = await userManager.FindByNameAsync(request.Username);
            if (user is null)
                return Results.Unauthorized();

            var valid = await userManager.CheckPasswordAsync(user, request.Password);
            if (!valid)
            {
                return Results.Unauthorized();
            }

            var token = issuer.IssueAdminToken(user.Id, user.UserName!);

            return Results.Ok(new TokenResponse(token));
        })
        .AllowAnonymous();

        return api;
    }
}
