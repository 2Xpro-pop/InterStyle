using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace InterStyle.AppHost;

public static class JwtExtensions
{

    public static IResourceBuilder<T> WithJwtAuthority<T>(this IResourceBuilder<T> builder, string authority) where T : IResourceWithEnvironment
    {
        return builder.WithEnvironment("Jwt__Authority", authority);
    }

    public static IResourceBuilder<T> WithJwtSigningKey<T>(
        this IResourceBuilder<T> builder,
        IResourceBuilder<ParameterResource> activeKid,
        IResourceBuilder<ParameterResource> pfxBase64,
        IResourceBuilder<ParameterResource> pfxPassword,
        int keyIndex = 0)
        where T : IResourceWithEnvironment
    {
        return builder
            .WithEnvironment($"Jwt__Signing__ActiveKid", activeKid)
            .WithEnvironment($"Jwt__Signing__Keys__{keyIndex}__Kid", activeKid)
            .WithEnvironment($"Jwt__Signing__Keys__{keyIndex}__PfxBase64", pfxBase64)
            .WithEnvironment($"Jwt__Signing__Keys__{keyIndex}__PfxPassword", pfxPassword);
    }

    public static IResourceBuilder<T> WithPublicJwtKey<T>(
        this IResourceBuilder<T> builder,
        IResourceBuilder<ParameterResource> pfxBase64,
        IResourceBuilder<ParameterResource> pfxPassword)
        where T : IResourceWithEnvironment
    {
        return builder.WithEnvironment(async context =>
        {
            var pfxBase64Value = await pfxBase64.Resource.GetValueAsync(context.CancellationToken);
            var pfxPasswordValue = await pfxPassword.Resource.GetValueAsync(context.CancellationToken);

            context.EnvironmentVariables["Jwt__PublicKey"] = ExtractPublicKeyPem(pfxBase64Value!, pfxPasswordValue);
        });
    }

    private static string ExtractPublicKeyPem(string pfxBase64, string? pfxPassword)
    {
        if (string.IsNullOrWhiteSpace(pfxBase64))
        {
            throw new InvalidOperationException("JWT signing PFX (base64) is not configured.");
        }

        var pfxBytes = Convert.FromBase64String(pfxBase64);
        using var certificate = X509CertificateLoader.LoadPkcs12(
            pfxBytes,
            pfxPassword,
            X509KeyStorageFlags.Exportable | X509KeyStorageFlags.EphemeralKeySet);

        using var rsa = certificate.GetRSAPublicKey();
        if (rsa is null)
        {
            throw new InvalidOperationException("The configured certificate does not contain an RSA public key.");
        }

        return rsa.ExportSubjectPublicKeyInfoPem();
    }
}
