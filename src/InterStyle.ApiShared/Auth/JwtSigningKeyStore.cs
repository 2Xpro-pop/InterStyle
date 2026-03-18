using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace InterStyle.ApiShared.Auth;

public sealed class JwtSigningKeyStore
{
    private readonly Dictionary<string, X509Certificate2> _certs = new(StringComparer.Ordinal);
    public string ActiveKid { get; }

    public JwtSigningKeyStore(IOptions<JwtOptions> opts)
    {
        var jwt = opts.Value;

        if (jwt.Signing is null)
            throw new InvalidOperationException("Jwt:Signing is not configured");

        if (string.IsNullOrWhiteSpace(jwt.Signing.ActiveKid))
            throw new InvalidOperationException("Jwt:Signing:ActiveKid is not configured");

        if (jwt.Signing.Keys.Count == 0)
            throw new InvalidOperationException("Jwt:Signing:Keys is empty");

        foreach (var k in jwt.Signing.Keys)
        {
            if (string.IsNullOrWhiteSpace(k.Kid))
            {
                throw new InvalidOperationException("Jwt:Signing:Keys[].Kid is required");
            }

            if (string.IsNullOrWhiteSpace(k.PfxBase64))
            {
                throw new InvalidOperationException($"Jwt:Signing:Keys[{k.Kid}].PfxBase64 is required");
            }

            var bytes = Convert.FromBase64String(k.PfxBase64);

            var cert = X509CertificateLoader.LoadPkcs12(bytes, k.PfxPassword);
            _certs[k.Kid] = cert;
        }

        ActiveKid = jwt.Signing.ActiveKid;

        if (!_certs.ContainsKey(ActiveKid))
            throw new InvalidOperationException("Jwt:Signing:ActiveKid not found among Jwt:Signing:Keys");
    }

    public X509Certificate2 GetActiveCert() => _certs[ActiveKid];

    public IReadOnlyDictionary<string, X509Certificate2> GetAll() => _certs;
}