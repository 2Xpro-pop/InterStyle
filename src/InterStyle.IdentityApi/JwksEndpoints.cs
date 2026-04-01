using System.Diagnostics;
using InterStyle.ApiShared;
using InterStyle.ApiShared.Auth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;

namespace InterStyle.IdentityApi;

public static class JwksEndpoints
{
    private static readonly ActivitySource ActivitySource = new("InterStyle.IdentityApi");

    public static IEndpointRouteBuilder MapJwks(this IEndpointRouteBuilder app)
    {
        app.MapGet("/.well-known/openid-configuration", (IOptions<JwtOptions> opts, ILoggerFactory loggerFactory) =>
        {
            using var activity = ActivitySource.StartActivity("GetOpenIdConfiguration");
            var logger = loggerFactory.CreateLogger("JwksEndpoints");

            var issuer = opts.Value.Issuer.TrimEnd('/');

            activity?.AddTag("identity.issuer", issuer);
            logger.LogInformation("Returning OpenID configuration for issuer {Issuer}", issuer);

            return Results.Json(new
            {
                issuer,
                jwks_uri = $"{issuer}/.well-known/jwks.json"
            });
        })
        .AllowAnonymous();

        app.MapGet("/.well-known/jwks.json", (JwtSigningKeyStore store, ILoggerFactory loggerFactory) =>
        {
            using var activity = ActivitySource.StartActivity("GetJwks");
            var logger = loggerFactory.CreateLogger("JwksEndpoints");

            try
            {
                var keys = store.GetAll()
                    .Select(pair =>
                    {
                        var kid = pair.Key;
                        var cert = pair.Value;

                        using var rsa = cert.GetRSAPublicKey();
                        if (rsa is null)
                        {
                            throw new InvalidOperationException($"Certificate '{kid}' does not contain RSA public key.");
                        }

                        var parameters = rsa.ExportParameters(false);

                        return new
                        {
                            kty = "RSA",
                            use = "sig",
                            kid,
                            alg = "RS256",
                            n = Base64UrlEncoder.Encode(parameters.Modulus!),
                            e = Base64UrlEncoder.Encode(parameters.Exponent!),
                            x5c = new[]
                            {
                                Convert.ToBase64String(cert.RawData)
                            },
                            x5t = Base64UrlEncoder.Encode(cert.GetCertHash())
                        };
                    })
                    .ToArray();

                activity?.AddTag("jwks.key_count", keys.Length);
                logger.LogInformation("Returning {KeyCount} JWKS key(s)", keys.Length);

                return Results.Json(new { keys });
            }
            catch (Exception ex)
            {
                activity?.SetExceptionTags(ex);
                logger.LogError(ex, "Failed to retrieve JWKS keys");
                throw;
            }
        })
        .AllowAnonymous();

        return app;
    }
}