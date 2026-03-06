using System;
using System.Collections.Generic;
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
}
