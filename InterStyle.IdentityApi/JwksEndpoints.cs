using InterStyle.ApiShared.Auth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;

namespace InterStyle.IdentityApi;

public static class JwksEndpoints
{
    public static IEndpointRouteBuilder MapJwks(this IEndpointRouteBuilder app)
    {
        app.MapGet("/.well-known/openid-configuration", (IOptions<JwtOptions> opts) =>
        {
            var issuer = opts.Value.Issuer.TrimEnd('/');

            return Results.Json(new
            {
                issuer,
                jwks_uri = $"{issuer}/.well-known/jwks.json"
            });
        })
        .AllowAnonymous();

        app.MapGet("/.well-known/jwks.json", (JwtSigningKeyStore store) =>
        {
            var jwks = store.GetAll()
                .Select(pair =>
                {
                    var kid = pair.Key;
                    var cert = pair.Value;

                    var key = new X509SecurityKey(cert) { KeyId = kid };

                    var jwk = JsonWebKeyConverter.ConvertFromX509SecurityKey(key);
                    jwk.Use = "sig";
                    jwk.Alg = "RS256";
                    jwk.Kid = kid;

                    return jwk;
                })
                .ToArray();

            return Results.Json(new { keys = jwks });
        })
        .AllowAnonymous();

        return app;
    }
}